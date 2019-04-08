using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    /// <summary>
    /// Send Message To Queue
    /// Send Message To Exchange
    /// </summary>
    public class RabbitMQManager<T> : IDisposable where T : IMessageTargetValidate
    {
        #region Properties
        public string VirtualHost { get; private set; }
        public string HostName { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public string Exchange { get; private set; }

        /// <summary>
        /// 消息发送目标类型
        /// </summary>
        public MessagePublishTypes PublishType {
            get {
                return typeof(T) == typeof(MessageTargetParamQueue) ? MessagePublishTypes.Queue : MessagePublishTypes.Exchange;
            }
        }

        private T ManagerParam { get; set; }
        #endregion

        protected IModel channel = null;
        protected ConnectionFactory factory = null;
        protected IConnection connection = null;

        public void Init()
        {
            factory = new ConnectionFactory()
            {
                HostName = this.HostName
            };
            if (!string.IsNullOrWhiteSpace(this.VirtualHost))
            {
                factory.VirtualHost = this.VirtualHost;
            }
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            if (PublishType == MessagePublishTypes.Queue)
            {
                var param = ManagerParam as MessageTargetParamQueue;
                Exchange= "";
                channel.QueueDeclare(queue: param.QueueName,
                    durable: param.Durable,
                    exclusive:param.Exclusive,
                    autoDelete:param.AutoDelete,
                    arguments:param.Arguments);
            }
            else
            {
                var param = ManagerParam as MessageTargetParamExchange;
                channel.ExchangeDeclare(exchange:param.ExchangeName,type:param.ExchangeType.ToString());
            }
        }

        private RabbitMQManager() { }

        public static RabbitMQManager<T> Create(T mgrParam, string host, string vHost, string userName="",string password="")
        {
            var mgr = new RabbitMQManager<T>
            {
                ManagerParam=mgrParam,
                HostName = host,
                UserName=userName,
                VirtualHost = vHost,
                Password=password
            };
            mgr.Init();
            return mgr;
        }

        public void SendMessage(RabbitMQMessage message)
        {
            if (PublishType == MessagePublishTypes.Queue) {
                message.RoutingKey = (ManagerParam as MessageTargetParamQueue).QueueName;
            }
            message.Send(channel, this.Exchange);
        }

        public void Dispose()
        {
            if (channel != null) channel.Dispose();
            if (connection != null) connection.Dispose();
            if (factory != null) factory = null;
        }
    }

    public interface IMessageTargetValidate
    {
        void Validate();
    }

    public class MessageTargetParamQueue:IMessageTargetValidate
    {
        public string QueueName { get; set; }
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public Dictionary<string, Object> Arguments { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(QueueName))
            {
                throw new ArgumentNullException("QueueName");
            }
        }
    }

    public class MessageTargetParamExchange : IMessageTargetValidate
    {

        /// <summary>
        /// 
        /// </summary>
        public string ExchangeName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ExchangeTypes ExchangeType { get; private set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ExchangeName))
            {
                throw new ArgumentNullException("ExchangeName");
            }
            if (ExchangeType == ExchangeTypes.None) {

                throw new ArgumentNullException("ExchangeType");
            }
        }
    }

    public class RabbitMQMessage
    {
        #region Properities
        /// <summary>
        /// 1,2 default 2
        /// </summary>
        public DeliveryModes DeliveryMode { get;protected set; }

        public string RoutingKey { get; set; }

        public Dictionary<string, object> Headers { get; protected set; }

        public Dictionary<MessageProperties, string> Properties { get; protected set; }

        public string Body { get; protected set; }
        #endregion

        private RabbitMQMessage() { }

        public static RabbitMQMessage Create(string body)
        {
            return Create(body,"", DeliveryModes.Non_persistent,null,new Dictionary<MessageProperties, string>() { { MessageProperties.content_type, "application/json" } });
        }

        public static RabbitMQMessage Create(string body, string routkey = "", Dictionary<string, object> headers = null, Dictionary<MessageProperties, string> props = null)
        {
            return Create(body,routkey,DeliveryModes.Non_persistent,headers,props);
        }

        public static RabbitMQMessage Create(string body,string routkey = "", DeliveryModes deliveryMode=DeliveryModes.Non_persistent, 
            Dictionary<string, object> headers=null, Dictionary<MessageProperties, string> props=null) {
            return new RabbitMQMessage() {
                Body = body,
                RoutingKey = routkey,
                Headers = headers,
                DeliveryMode= deliveryMode,
                Properties = props
            };
        }

        public IBasicProperties GetBasicProp(IModel model)
        {
            var prop = model.CreateBasicProperties();
            prop.DeliveryMode = (byte)DeliveryMode;
            //Proc Properties
            if (Properties != null && Properties.Count > 0)
            {
                foreach (var key in this.Properties.Keys)
                {
                    switch (key)
                    {
                        case MessageProperties.content_type:
                            prop.ContentType = Properties[MessageProperties.content_type];
                            break;
                        case MessageProperties.content_encoding:
                            prop.ContentEncoding = Properties[MessageProperties.content_encoding];
                            break;
                        case MessageProperties.correlation_id:
                            prop.CorrelationId = Properties[MessageProperties.correlation_id];
                            break;
                        case MessageProperties.reply_to:
                            prop.ReplyTo = Properties[MessageProperties.reply_to];
                            break;
                        case MessageProperties.expiration:
                            prop.Expiration = Properties[MessageProperties.expiration];
                            break;
                        case MessageProperties.message_id:
                            prop.MessageId = Properties[MessageProperties.message_id];
                            break;
                        case MessageProperties.type:
                            prop.Type = Properties[MessageProperties.type];
                            break;
                        case MessageProperties.user_id:
                            prop.UserId = Properties[MessageProperties.user_id];
                            break;
                        case MessageProperties.app_id:
                            prop.AppId = Properties[MessageProperties.app_id];
                            break;
                        case MessageProperties.cluster_id:
                            prop.ClusterId = Properties[MessageProperties.cluster_id];
                            break;
                            /*
                            case MessageProperties.timestamp:
                            prop.Timestamp = Properties[MessageProperties.timestamp];
                            break;
                            case MessageProperties.priority:
                            prop.Priority = (byte)Properties[MessageProperties.priority];
                            break;
                            */
                    }
                }
            }

            //Proc Headers
            if (Headers != null && Headers.Count > 0)
            {
                foreach (var key in Headers.Keys)
                {
                    prop.Headers.Add(key, Headers[key]);
                }
            }
            return prop;
        }

        public void Send(IModel channel, string exchange)
        {
            channel.BasicPublish(
                exchange: exchange,
                routingKey: RoutingKey,
                basicProperties: GetBasicProp(channel),
                body: Encoding.UTF8.GetBytes(this.Body)
            );
        }
    }

    public enum MessagePublishTypes {
        Exchange,
        Queue
    }

    /// <summary>
    /// Exchange Types
    /// </summary>
    public enum ExchangeTypes
    {
        None,
        direct,
        topic,
        headers,
        fanout
    }

    public enum DeliveryModes : byte
    {
        /// <summary>
        /// Non-persistent
        /// </summary>
        Non_persistent = 1,
        Persistent = 2
    }

    public enum MessageProperties
    {
        content_type,
        content_encoding,
        priority,
        correlation_id,
        reply_to,
        expiration,
        message_id,
        timestamp,
        type,
        user_id,
        app_id,
        cluster_id
    }

    #region DefaultHeaders
    public class DictionaryEx<TKey, TValue> : Dictionary<TKey,TValue>
    {
        public DictionaryEx<TKey, TValue> AddEx(TKey key,TValue value)
        {
            if (this.ContainsKey(key))
            {
                this[key] = value;
            }
            else
            {
                this[key] = value;
            }
            return this;
        }
    }

    /// <summary>
    /// Predefined Params for RabbitMqHelper
    /// </summary>
    public static class PredefinedParams {

        /// <summary>
        /// With Content Type application/json
        /// content Encoding utf-8
        /// </summary>
        public static DictionaryEx<MessageProperties, string> PropJsonMessage = new DictionaryEx<MessageProperties, string>() {
            { MessageProperties.content_type,"application/json"},
            { MessageProperties.content_encoding,"utf-8"},
        };

        public static DictionaryEx<string, string> Header4HinaStudy = new DictionaryEx<string, string>() {
            {"category","" },
            {"version","" },
            {"","" }
        };
    }
    #endregion
}
