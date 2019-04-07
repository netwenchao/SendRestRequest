using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace CommonLib
{
    public class ConfigFileUtils
    {
         /// <summary>
         /// 验证指定目录下所有.config中连接字符串的正确性
         /// </summary>
         /// <param name="path"></param>
         /// <returns></returns>
        public static ResponseMessage<bool> ValidateConnectionStrInConfigFile(string path,bool showConfigWithoutFileName=false)
        {
            var rslt = new ResponseMessage<bool>() { Success = true };
            var filePattern = "*.config";

            try
            {
                if (!Directory.Exists(path))
                {
                    return rslt.Failed("Direcotry({0}) is not exists.", path);
                }
                var files = Directory.EnumerateFiles(path, filePattern, SearchOption.AllDirectories).ToList();
                var configFiles = new List<ConfigFile>();

                //Create Config file and load Connection string.
                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        configFiles.Add(new ConfigFile(file).LoadConnectionStrings());
                    }
                }
                else
                {
                    rslt.Message = "No ConfigFile is founded.";
                    return rslt;
                }

                //Do Test Connection
                var successConns = new List<string>();
                foreach (var cfgFile in configFiles)
                {
                    if (cfgFile.ConnectionStrings != null && cfgFile.ConnectionStrings.Count > 0)
                    {
                        foreach (var conn in cfgFile.ConnectionStrings)
                        {
                            if (!successConns.Contains(conn.ConnectionString))
                            {
                                var connRslt = conn.TestConnection();
                                if (connRslt.HasValue && connRslt.Value)
                                {
                                    successConns.Add(conn.ConnectionString);
                                }
                            }
                            else
                            {
                                conn.SetSuccess();
                            }
                        }
                    }
                }

                var messageBuilder = new StringBuilder();
                var hasConnConfigs = configFiles.Where(t => t.HasConnectionStrings);
                var successCount = hasConnConfigs.Sum(c => c.SuccessCount);
                var totalCount= hasConnConfigs.Sum(c => c.ConnectionStringCount);
                messageBuilder.AppendFormat("Success:{0},Failed:{1}\r\n", successCount, totalCount-successCount);

                //Show Failed
                var failed = hasConnConfigs.Where(f => f.HasFailed);
                if (failed.Any())
                {                     
                    messageBuilder.Append("\r\n==============Connection Failed==================");                
                    foreach (var file in failed)
                    {
                        AppendLogForConfigFile(messageBuilder,file);
                    }
                }

                //Show Success
                var success = hasConnConfigs.Where(f => !f.HasFailed);
                if (success.Any())
                {                   
                    messageBuilder.Append("\r\n==============Connection Success==================");
                    foreach (var file in success)
                    {
                        AppendLogForConfigFile(messageBuilder, file);
                    }
                }

                //Show without Connections
                if (showConfigWithoutFileName)
                {
                    messageBuilder.Append("\r\n==============Config file without connectionstring.==================");
                    foreach (var file in configFiles.Where(c => !c.HasConnectionStrings))
                    {
                        messageBuilder.Append(file.FileName+ "\r\n");
                    }
                }

                rslt.Success = (totalCount - successCount) == 0;
                rslt.Message = messageBuilder.ToString();
            }
            catch (Exception exp)
            {
                rslt.Failed(exp.ToString());
            }
            return rslt;
        }

        private static void AppendLogForConfigFile(StringBuilder messageBuilder, ConfigFile file)
        {
            messageBuilder.AppendFormat("\r\n{0}",file.FileName);
            foreach (var conn in file.ConnectionStrings.OrderBy(t => t.IsAvailable))
            {
                if (conn.IsAvailable.HasValue && conn.IsAvailable.Value)
                {
                    messageBuilder.AppendFormat("\t\r\n{0} Name:{1}\r\n\t\tProviderName:{2}\r\n\t\tConnectionString:{3}",
                        conn.IsAvailable.HasValue && conn.IsAvailable.Value ? "[Ok]" : "[Failed]",
                        conn.Name,
                        conn.ProvderName,
                        conn.ConnectionString);
                }
                else {
                    messageBuilder.AppendFormat("\t\r\n{0} Name:{1}\r\n\t\tProviderName:{2}\r\n\t\tConnectionString:{3}\r\n\t\tException:{4}",
                        conn.IsAvailable.HasValue && conn.IsAvailable.Value ? "[Ok]" : "[Failed]",
                        conn.Name,
                        conn.ProvderName,
                        conn.ConnectionString,
                        conn.Message);
                }
            }
        }
    }

    public class ConfigFile
    {
        public ConfigFile(string fileName)
        {
            this.FileName = fileName;
        }

        public string FileName { get; set; }
        public string Message { get; protected set; }

        public List<ConnEntity> ConnectionStrings { get; protected set; }

        public bool HasConnectionStrings { get { return ConnectionStrings != null && ConnectionStrings.Count >0; } }

        public int ConnectionStringCount { get { return HasConnectionStrings ? ConnectionStrings.Count : 0; } }

        public int SuccessCount { get { return HasConnectionStrings ? ConnectionStrings.Count(t => t.IsAvailable.HasValue && t.IsAvailable.Value) : 0; } }

        public bool HasFailed { get {return ConnectionStringCount - SuccessCount > 0;} }

        public ConfigFile LoadConnectionStrings()
        {
            var xpath = @"//connectionStrings/add";
            this.ConnectionStrings = new List<ConnEntity>();
            if (File.Exists(FileName))
            {
                try
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.Load(this.FileName);
                    var connNodes = xmlDoc.SelectNodes(xpath);
                    if (connNodes.Count > 0)
                    {
                        foreach (XmlNode node in connNodes)
                        {
                            var cfg = ConnEntity.LoadFromXmlNode(node);
                            if (cfg != null)
                            {
                                ConnectionStrings.Add(cfg);
                            }
                        }
                    }
                    else
                    {
                        this.Message = "Can't find connectionstring by xpath " + xpath;
                    }
                }
                catch (Exception exp)
                {
                    this.Message = exp.ToString();
                }
            }
            else
            {
                this.Message = "File is not exists.";
            }
            return this;
        }

        public class ConnEntity
        {
            public string Name { get; set; }
            public string ProvderName { get; set; }
            public string ConnectionString { get; set; }
            /// <summary>
            /// 是否可以连通
            /// </summary>
            public bool? IsAvailable { get; protected set; }
            /// <summary>
            /// 原因
            /// </summary>
            public string Message { get; protected set; }

            public static ConnEntity LoadFromXmlNode(XmlNode node)
            {
                var nameNode = node.Attributes["name"];
                var providerNode = node.Attributes["providerName"];
                var connNode = node.Attributes["connectionString"];
                if (connNode != null || nameNode != null || providerNode != null)
                {
                    return new ConnEntity()
                    {
                        Name = nameNode == null ? "" : nameNode.Value,
                        ProvderName = providerNode == null ? "" : providerNode.Value,
                        ConnectionString = connNode == null ? "" : connNode.Value
                    };
                }
                return null;
            }

            /// <summary>
            /// 测试连接
            /// </summary>
            /// <returns></returns>
            public bool? TestConnection()
            {
                if (string.IsNullOrWhiteSpace(ConnectionString))
                {
                    IsAvailable = false;
                    Message = "Connection String is null or whitespace.";
                    return IsAvailable;
                }

                SqlConnection conn = null;
                try
                {
                    conn = new SqlConnection(ConnectionString);
                    conn.Open();
                    IsAvailable = true;
                }
                catch (Exception exp)
                {
                    IsAvailable = false;
                    Message = exp.Message;
                }
                finally
                {
                    if (conn == null && conn.State == ConnectionState.Open)
                    {
                        try
                        {
                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                return IsAvailable;
            }

            /// <summary>
            /// 成功
            /// </summary>
            public void SetSuccess()
            {
                IsAvailable = true;
            }
        }
    }

    public class ResponseMessage
    {

        public bool Success { get; set; }
        public string Message { get; set; }

        public ResponseMessage()
        {
            Success = true;
        }
    }

    public class ResponseMessage<T> : ResponseMessage
    {
        public T Data { get; set; }

        public ResponseMessage<T> Failed(string message)
        {
            this.Success = false;
            this.Message = message;
            return this;
        }

        public ResponseMessage<T> Failed(string messagePattern, params object[] strParam)
        {
            return Failed(string.Format(messagePattern, strParam));
        }
    }
}
