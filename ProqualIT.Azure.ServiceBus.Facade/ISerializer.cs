using System;
using System.IO;

namespace ProqualIT.Azure.ServiceBus.Facade
{
    /// <summary>
    /// Interface that must be implemented to register a custom serializer
    /// </summary>
    public interface ISerializer : IDisposable
    {
        
        /// <summary>
        /// Create an instance of the serializer
        /// </summary>
        /// <returns></returns>
        ISerializer Create();

        /// <summary>
        /// Serialize the message
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        Stream Serialize(object obj);

        /// <summary>
        /// Deserialize the message
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        object Deserialize(Stream stream, Type type);

        /// <summary>
        /// Deserialize the message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        T Deserialize<T>(Stream stream);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        T Deserialize<T>(string stringValue);
    }
}
