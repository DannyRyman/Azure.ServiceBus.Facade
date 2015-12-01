using System;
using System.IO;
using Newtonsoft.Json;

namespace ProqualIT.Azure.ServiceBus.Facade
{
    /// <summary>
    /// JSon Serializer that may be used to serialize messages
    /// </summary>
    public class JsonSerializer : ISerializer
    {
        private Stream _serializedStream;

        #region ISerializer Members
        
        /// <summary>
        /// Create an instance of the serializer
        /// </summary>
        /// <returns></returns>
        public ISerializer Create()
        {
            return new JsonSerializer();
        }

        /// <summary>
        /// Serialize the message
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Stream Serialize(object obj)
        {
            var serializer = CreateJsonSerializer();

            _serializedStream = new MemoryStream();

            var sw = new StreamWriter(_serializedStream);
            //do not wrap in using, we don't want to close the stream
            JsonWriter jw = new JsonTextWriter(sw);
            serializer.Serialize(jw, obj);
            jw.Flush();
            _serializedStream.Position = 0;
            //make sure you always set the stream position to where you want to serialize.
            return _serializedStream;
        }
        
        /// <summary>
        /// Deserialize the message
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Deserialize(Stream stream, Type type)
        {
            var serializer = CreateJsonSerializer();

            var sr = new StreamReader(stream);
            //do not wrap in using, we don't want to close the stream
            var jr = new JsonTextReader(sr);
            return serializer.Deserialize(jr, type);
        }

        /// <summary>
        /// Deserialize the message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public T Deserialize<T>(Stream stream)
        {
            return (T) Deserialize(stream, typeof (T));
        }

        /// <summary>
        /// Deserialize the message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        public T Deserialize<T>(string stringValue)
        {
            return (T)Deserialize(stringValue, typeof(T));
        }


        /// <summary>
        /// Deserialize the message
        /// </summary>
        /// <param name="stringValue"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Deserialize(string stringValue, Type type)
        {
            var serializer = CreateJsonSerializer();

            var sr = new StringReader(stringValue);
            //do not wrap in using, we don't want to close the stream
            var jr = new JsonTextReader(sr);
            return serializer.Deserialize(jr, type);
        }


        /// <summary>
        /// Serializer Settings
        /// </summary>
        public JsonSerializerSettings SerializerSettings
        {
            get
            {
                return new GuidelineJsonSerializerSettings();
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (_serializedStream != null)
            {
                _serializedStream.Dispose();
                _serializedStream = null;
            }
        }
        #endregion

        #region Private Methods

        private static Newtonsoft.Json.JsonSerializer CreateJsonSerializer()
        {  
            return Newtonsoft.Json.JsonSerializer.Create(new GuidelineJsonSerializerSettings());
        }
        #endregion
    }
}