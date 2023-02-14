
//Created by Alexander Fields 

namespace Optimization.Objects.Logging
{
    /// <summary>
    /// Log Message Object for easier logging
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public LogMessage()
        {
        }
        /// <summary>
        /// Partial Constructor
        /// </summary>
        /// <param name="localOperationName"></param>
        /// <param name="messageType"></param>
        /// <param name="message"></param>
        public LogMessage(string localOperationName, MessageType messageType, string message)
        {
            TimeStamp = System.DateTime.Now;
            MessageSource = MessageSourceSetter;
            LocalOperationName = localOperationName;
            MessageType = messageType;
            Message = message;
        }
        /// <summary>
        /// Full Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="timeStamp"></param>
        /// <param name="localOperationName"></param>
        /// <param name="messageType"></param>
        /// <param name="message"></param>
        public LogMessage(int id, System.DateTime timeStamp, string localOperationName, MessageType messageType, string message)
        {
            Id = id;
            TimeStamp = timeStamp;
            MessageSource = MessageSourceSetter;
            LocalOperationName = localOperationName; 
            MessageType = messageType;
            Message = message;
        }

        public long Id { get; set; }
        public System.DateTime TimeStamp { get; set; }
        /// <summary>
        /// This is the program that is running
        /// </summary>
        public static string MessageSourceSetter { get; set; }
        public string MessageSource { get; set; }
        public string LocalOperationName { get; set; }
        public MessageType MessageType { get; set; }
        public string Message { get; set; }
    }
}
