using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// comparison of xml and xml data contract serialization:
// http://web.archive.org/web/20130430190551/http://www.danrigsby.com/blog/index.php/2008/03/07/xmlserializer-vs-datacontractserializer-serialization-in-wcf/

// xml data contract serialization:
// https://docs.microsoft.com/en-us/dotnet/api/system.runtime.serialization.datacontractserializer?view=netframework-4.7.2

// When loading a derived class from file into a base class handle named Obj...
// ... using binary: Obj.GetType() returns the type of the derived class. Derived class was loaded from file.
// ... using xml or xml data contract: throws an error due to type mismatch
// ... using json: Obj.GetType() returns the type of the base class. The BASE class members were loaded from the derived class on-file, and the derived-class-only members were discarded (they remain intact on-file).

public class LocalDataManager : SingletonDDOL<LocalDataManager>
{
    public enum SerializationType { binary, xml, xmlDataContract, json }

    /// <summary>
    /// Whether json serialization should be saved using extra whitespace and newlines for human readability.
    /// </summary>
    public bool useJsonPretty = false;

    /// <summary>
    /// Attempts to save an object to a local file in the application's persistent datapath. The object must be serializable or use the [DataContract] attribute.
    /// </summary>
    /// <param name="filename">Name and extension of the file to save to. If it doesn't exist, it will be created.</param>
    /// <param name="data">The object to save to file.</param>
    /// <param name="serializationType">The serialization method to use when saving to file. Make sure to use the appropriate file extension to make the files easier to identify.</param>
    /// <param name="shouldRethrowErrors">Whether encountered exceptions should be rethrown.</param>
    /// <returns>Returns true if the process succeeds, else returns false.</returns>
    public bool SaveObjectToFile(string filename, object data, SerializationType serializationType = SerializationType.binary, bool shouldRethrowErrors = false)
    {
        string fullpath = Application.persistentDataPath + "/" + filename;

        if (serializationType != SerializationType.xmlDataContract && !data.GetType().IsSerializable)
        {
            Debug.LogError("Attempted to save the non-serializable data \"" + data.ToString() + "\" of type " + data.GetType().ToString() + " to " + fullpath + ". Failed to save file.");
            return false;
        }
        else if (serializationType == SerializationType.xmlDataContract && !(data.GetType().GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0))
        {
            Debug.LogError("Attempted to save the data \"" + data.ToString() + "\" of type " + data.GetType().ToString() + ", which does not use the [DataContract] attribute, to " + fullpath + " using a data contract serializer. Failed to save file.");
            return false;
        }

        FileStream fs = new FileStream(fullpath, FileMode.Create);

        try
        {
            switch (serializationType)
            {
                case SerializationType.binary:
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, data);
                    break;

                case SerializationType.xml:
                    System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(data.GetType());
                    xmlSerializer.Serialize(fs, data);
                    break;

                case SerializationType.xmlDataContract:
                    DataContractSerializer dataContractSerializer = new DataContractSerializer(data.GetType());
                    dataContractSerializer.WriteObject(fs, data);
                    break;

                case SerializationType.json:
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        string jsonOutput = JsonUtility.ToJson(data, useJsonPretty);
                        sw.Write(jsonOutput);
                    }
                    break;

                default:
                    Debug.LogError("Attempted to use invalid serialization type " + serializationType.ToString() + " when saving " + fullpath + ". Process aborted.");

                    if (fs != null)
                    {
                        fs.Close();
                    }

                    try
                    {
                        fs.Dispose();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                    }

                    return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to write to " + fullpath + ". Reason: " + e.Message);

            if (fs != null)
            {
                fs.Close();
            }

            try
            {
                fs.Dispose();
            }
            catch (Exception e2)
            {
                Debug.LogWarning(e2.Message);
            }

            if (shouldRethrowErrors)
            {
                throw e;
            }

            return false;
        }

        if (DebugManager.instance.enableDebug && DebugManager.instance.debugSerializationSuccessMessages)
        {
            Debug.Log("Saved data successfully to " + fullpath);
        }

        if (fs != null)
        {
            fs.Close();
        }

        try
        {
            fs.Dispose();
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }        

        return true;
    }

    /// <summary>
    /// Attempts to save an object to a local file in the application's persistent datapath. The object must be serializable or use the [DataContract] attribute.
    /// </summary>
    /// <param name="filepath">Filepath to the location of the file. This filepath will be appended to the application's persistent datapath, and it will be created if it does not exist.</param>
    /// <param name="filename">Name and extension of the file to save to. If it doesn't exist, it will be created.</param>
    /// <param name="data">The object to save to file.</param>
    /// <param name="serializationType">The serialization method to use when saving to file. Make sure to use the appropriate file extension to make the files easier to identify.</param>
    /// <param name="shouldRethrowErrors">Whether encountered exceptions should be rethrown.</param>
    /// <returns>Returns true if the process succeeds, else returns false.</returns>
    public bool SaveObjectToFile(string filepath, string filename, object data, SerializationType serializationType = SerializationType.binary, bool shouldRethrowErrors = false)
    {
        string fullpath = Application.persistentDataPath + "/" + filepath + "/" + filename;

        if (serializationType != SerializationType.xmlDataContract && !data.GetType().IsSerializable)
        {
            Debug.LogError("Attempted to save the non-serializable data \"" + data.ToString() + "\" of type " + data.GetType().ToString() + " to " + fullpath + ". Failed to save file.");
            return false;
        }
        else if (serializationType == SerializationType.xmlDataContract && !(data.GetType().GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0))
        {
            Debug.LogError("Attempted to save the data \"" + data.ToString() + "\" of type " + data.GetType().ToString() + ", which does not use the [DataContract] attribute, to " + fullpath + " using a data contract serializer. Failed to save file.");
            return false;
        }

        System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/" + filepath);
        FileStream fs = new FileStream(fullpath, FileMode.Create);

        try
        {
            switch (serializationType)
            {
                case SerializationType.binary:
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, data);
                    break;

                case SerializationType.xml:
                    System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(data.GetType());
                    xmlSerializer.Serialize(fs, data);
                    break;

                case SerializationType.xmlDataContract:
                    DataContractSerializer dataContractSerializer = new DataContractSerializer(data.GetType());
                    dataContractSerializer.WriteObject(fs, data);
                    break;

                case SerializationType.json:
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        string jsonOutput = JsonUtility.ToJson(data, useJsonPretty);
                        sw.Write(jsonOutput);
                    }
                    break;

                default:
                    Debug.LogError("Attempted to use invalid serialization type " + serializationType.ToString() + " when saving " + fullpath + ". Process aborted.");

                    if (fs != null)
                    {
                        fs.Close();
                    }

                    try
                    {
                        fs.Dispose();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                    }

                    return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to write to " + fullpath + ". Reason: " + e.Message);

            if (fs != null)
            {
                fs.Close();
            }

            try
            {
                fs.Dispose();
            }
            catch (Exception e2)
            {
                Debug.LogWarning(e2.Message);
            }

            if (shouldRethrowErrors)
            {
                throw e;
            }

            return false;
        }

        if (DebugManager.instance.debugSerializationSuccessMessages)
        {
            Debug.Log("Saved data successfully to " + fullpath);
        }



        if (fs != null)
        {
            fs.Close();
        }

        try
        {
            fs.Dispose();
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }
        

        return true;
    }

    /// <summary>
    /// Attempts to load an object of type T from a local file in the application's persistent datapath. T must be serializable or use the [DataContract] attribute.
    /// </summary>
    /// <typeparam name="T">Must be serializable or use the [DataContract] attribute.</typeparam>
    /// <param name="filename">Name and extension of the file to read from.</param>
    /// <param name="data">Reference to the object that will hold the data deserialized from the file, if the process succeeds.</param>
    /// <param name="serializationType">The deserialization method to use when retrieving data from the file. This must match the serialization method that was used to save the file.</param>
    /// <param name="shouldRethrowErrors">Whether encountered exceptions should be rethrown.</param>
    /// <returns>Returns true if the process succeeds, else returns false.</returns>
    public bool LoadObjectFromFile<T>(string filename, out T data, SerializationType serializationType = SerializationType.binary, bool shouldRethrowErrors = false) where T : class
    {
        string fullpath = Application.persistentDataPath + "/" + filename;

        if (serializationType != SerializationType.xmlDataContract && !typeof(T).IsSerializable)
        {
            Debug.LogError("Attempted to read the non-serializable data type " + typeof(T).ToString() + " from " + fullpath + ". Did not attempt to deserialize the requested file.");
            data = default(T);
            return false;
        }
        else if (serializationType == SerializationType.xmlDataContract && !(typeof(T).GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0))
        {
            Debug.LogError("Attempted to read the data type " + typeof(T).ToString() + " from " + fullpath + " using an xml data contract serializer, but the type " + typeof(T).ToString() + " does not use the [DataContract] attribute. Did not attempt to deserialize the requested file.");
            data = default(T);
            return false;
        }

        FileStream fs = null;

        try
        {
            fs = new FileStream(fullpath, FileMode.Open);
        }
        catch (Exception e)
        {
            Debug.LogError("Error opening " + fullpath + " for reading. Reason: " + e.Message);

            if (fs != null)
            {
                fs.Close();
            }

            try
            {
                fs.Dispose();
            }
            catch (Exception e2)
            {
                Debug.LogWarning(e2.Message);
            }

            data = default(T);

            if (shouldRethrowErrors)
            {
                throw e;
            }

            return false;
        }

        try
        {
            switch (serializationType)
            {
                case SerializationType.binary:
                    BinaryFormatter bf = new BinaryFormatter();
                    data = (T)bf.Deserialize(fs);
                    break;

                case SerializationType.xml:
                    System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    data = (T)xmlSerializer.Deserialize(fs);
                    break;

                case SerializationType.xmlDataContract:
                    DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(T));
                    data = (T)dataContractSerializer.ReadObject(fs);
                    break;

                case SerializationType.json:
                    using (StreamReader sw = new StreamReader(fs))
                    {
                        string jsonInput = sw.ReadToEnd();
                        data = JsonUtility.FromJson<T>(jsonInput);
                    }
                    break;

                default:
                    Debug.LogError("Attempted to use invalid serialization type " + serializationType.ToString() + " when reading " + fullpath + ". Process aborted.");

                    if (fs != null)
                    {
                        fs.Close();
                    }

                    try
                    {
                        fs.Dispose();
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e.Message);
                    }

                    data = default(T);

                    return false;
            }
        }
        catch (InvalidCastException e)
        {
            Debug.LogError("Error when attempting to cast file data from " + fullpath + " to the type " + typeof(T).ToString() + ". Reason: " + e.Message);

            if (fs != null)
            {
                fs.Close();
            }

            try
            {
                fs.Dispose();
            }
            catch (Exception e2)
            {
                Debug.LogWarning(e2.Message);
            }

            data = default(T);

            if (shouldRethrowErrors)
            {
                throw e;
            }

            return false;
        }
        catch (Exception e)
        {
            Debug.LogError("Error when attempting to deserialize data from " + fullpath + ". Reason: " + e.Message);

            if (fs != null)
            {
                fs.Close();
            }

            try
            {
                fs.Dispose();
            }
            catch (Exception e2)
            {
                Debug.LogWarning(e2.Message);
            }

            data = default(T);

            if (shouldRethrowErrors)
            {
                throw e;
            }

            return false;
        }

        if (DebugManager.instance.enableDebug && DebugManager.instance.debugSerializationSuccessMessages)
        {
            Debug.Log("Retrieved data successfully from " + fullpath);
        }

        if (fs != null)
        {
            fs.Close();
        }

        try
        {
            fs.Dispose();
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }

        return true;
    }

    /// <summary>
    /// Attempts to load an object of type T from a local file in the application's persistent datapath. T must be serializable or use the [DataContract] attribute.
    /// </summary>
    /// <typeparam name="T">Must be serializable or use the [DataContract] attribute.</typeparam>
    /// <param name="filepath">Filepath to the location of the file. This filepath will be appended to the application's persistent datapath.</param>
    /// <param name="filename">Name and extension of the file to read from.</param>
    /// <param name="data">Reference to the object that will hold the data deserialized from the file, if the process succeeds.</param>
    /// <param name="serializationType">The deserialization method to use when retrieving data from the file. This must match the serialization method that was used to save the file.</param>
    /// <param name="shouldRethrowErrors">Whether encountered exceptions should be rethrown.</param>
    /// <returns>Returns true if the process succeeds, else returns false.</returns>
    public bool LoadObjectFromFile<T>(string filepath, string filename, out T data, SerializationType serializationType = SerializationType.binary, bool shouldRethrowErrors = false) where T : class
    {       

        string fullpath = Application.persistentDataPath + "/" + filepath + "/" + filename;

        if (serializationType != SerializationType.xmlDataContract && !typeof(T).IsSerializable)
        {
            Debug.LogError("Attempted to read the non-serializable data type " + typeof(T).ToString() + " from " + fullpath + ". Did not attempt to deserialize the requested file.");
            data = default(T);
            return false;
        }
        else if (serializationType == SerializationType.xmlDataContract && !(typeof(T).GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0))
        {
            Debug.LogError("Attempted to read the data type " + typeof(T).ToString() + " from " + fullpath + " using an xml data contract serializer, but the type " + typeof(T).ToString() + " does not use the [DataContract] attribute. Did not attempt to deserialize the requested file.");
            data = default(T);
            return false;
        }

        FileStream fs = null;

        try
        {
            fs = new FileStream(fullpath, FileMode.Open);
        }
        catch (Exception e)
        {
            Debug.LogError("Error opening " + fullpath + " for reading. Reason: " + e.Message);

            if (fs != null)
            {
                fs.Close();
            }

            try
            {
                fs.Dispose();
            }
            catch (Exception e2)
            {
                Debug.LogWarning(e2.Message);
            }

            data = default(T);

            if (shouldRethrowErrors)
            {
                throw e;
            }

            return false;
        }

        try
        {
            switch (serializationType)
            {
                case SerializationType.binary:
                    BinaryFormatter bf = new BinaryFormatter();
                    data = (T)bf.Deserialize(fs);
                    break;

                case SerializationType.xml:
                    System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    data = (T)xmlSerializer.Deserialize(fs);
                    break;

                case SerializationType.xmlDataContract:
                    DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(T));
                    data = (T)dataContractSerializer.ReadObject(fs);
                    break;

                case SerializationType.json:
                    using (StreamReader sw = new StreamReader(fs))
                    {
                        string jsonInput = sw.ReadToEnd();
                        data = JsonUtility.FromJson<T>(jsonInput);
                    }
                    break;

                default:
                    Debug.LogError("Attempted to use invalid serialization type " + serializationType.ToString() + " when reading " + fullpath + ". Process aborted.");

                    if (fs != null)
                    {
                        fs.Close();
                    }

                    try
                    {
                        fs.Dispose();
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e.Message);
                    }

                    data = default(T);

                    return false;
            }
        }
        catch (InvalidCastException e)
        {
            Debug.LogError("Error when attempting to cast file data from " + fullpath + " to the type " + typeof(T).ToString() + ". Reason: " + e.Message);

            if (fs != null)
            {
                fs.Close();
            }

            try
            {
                fs.Dispose();
            }
            catch (Exception e2)
            {
                Debug.LogWarning(e2.Message);
            }

            data = default(T);

            if (shouldRethrowErrors)
            {
                throw e;
            }

            return false;
        }
        catch (Exception e)
        {
            Debug.LogError("Error when attempting to deserialize data from " + fullpath + ". Reason: " + e.Message);

            if (fs != null)
            {
                fs.Close();
            }

            try
            {
                fs.Dispose();
            }
            catch (Exception e2)
            {
                Debug.LogWarning(e2.Message);
            }

            data = default(T);

            if (shouldRethrowErrors)
            {
                throw e;
            }

            return false;
        }

        if (DebugManager.instance.debugSerializationSuccessMessages)
        {
            Debug.Log("Retrieved data successfully from " + fullpath);
        }

        if (fs != null)
        {
            fs.Close();
        }

        try
        {
            fs.Dispose();
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }

        return true;
    }

    /// <summary>
    /// Attempts to load an object of type T from a local file in the application's persistent datapath. T must be serializable or use the [DataContract] attribute.
    /// </summary>
    /// <typeparam name="T">Must be serializable or use the [DataContract] attribute.</typeparam>
    /// <param name="filename">Name and extension of the file to read from.</param>
    /// <param name="data">Reference to the object that will hold the data deserialized from the file, if the process succeeds.</param>
    /// <param name="serializationType">The deserialization method to use when retrieving data from the file. This must match the serialization method that was used to save the file.</param>
    /// <param name="shouldRethrowErrors">Whether encountered exceptions should be rethrown.</param>
    /// <returns>Returns true if the process succeeds, else returns false.</returns>
    public bool LoadObjectFromFile(string filename, out object data, SerializationType serializationType = SerializationType.binary, bool shouldRethrowErrors = false)
    {
        string fullpath = Application.persistentDataPath + "/" + filename;

        if (serializationType != SerializationType.xmlDataContract && !typeof(object).IsSerializable)
        {
            Debug.LogError("Attempted to read the non-serializable data type " + typeof(object).ToString() + " from " + fullpath + ". Did not attempt to deserialize the requested file.");
            data = default(object);
            return false;
        }
        else if (serializationType == SerializationType.xmlDataContract && !(typeof(object).GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0))
        {
            Debug.LogError("Attempted to read the data type " + typeof(object).ToString() + " from " + fullpath + " using an xml data contract serializer, but the type " + typeof(object).ToString() + " does not use the [DataContract] attribute. Did not attempt to deserialize the requested file.");
            data = default(object);
            return false;
        }

        FileStream fs = null;

        try
        {
            fs = new FileStream(fullpath, FileMode.Open);
        }
        catch (Exception e)
        {
            Debug.LogError("Error opening " + fullpath + " for reading. Reason: " + e.Message);

            if (fs != null)
            {
                fs.Close();
            }

            try
            {
                fs.Dispose();
            }
            catch (Exception e2)
            {
                Debug.LogWarning(e2.Message);
            }

            data = default(object);

            if (shouldRethrowErrors)
            {
                throw e;
            }

            return false;
        }

        try
        {
            switch (serializationType)
            {
                case SerializationType.binary:
                    BinaryFormatter bf = new BinaryFormatter();
                    data = (object)bf.Deserialize(fs);
                    break;

                case SerializationType.xml:
                    System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(object));
                    data = (object)xmlSerializer.Deserialize(fs);
                    break;

                case SerializationType.xmlDataContract:
                    DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(object));
                    data = (object)dataContractSerializer.ReadObject(fs);
                    break;

                case SerializationType.json:
                    using (StreamReader sw = new StreamReader(fs))
                    {
                        string jsonInput = sw.ReadToEnd();
                        data = JsonUtility.FromJson<object>(jsonInput);
                    }
                    break;

                default:
                    Debug.LogError("Attempted to use invalid serialization type " + serializationType.ToString() + " when reading " + fullpath + ". Process aborted.");

                    if (fs != null)
                    {
                        fs.Close();
                    }

                    try
                    {
                        fs.Dispose();
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e.Message);
                    }

                    data = default(object);

                    return false;
            }
        }
        catch (InvalidCastException e)
        {
            Debug.LogError("Error when attempting to cast file data from " + fullpath + " to the type " + typeof(object).ToString() + ". Reason: " + e.Message);

            if (fs != null)
            {
                fs.Close();
            }

            try
            {
                fs.Dispose();
            }
            catch (Exception e2)
            {
                Debug.LogWarning(e2.Message);
            }

            data = default(object);

            if (shouldRethrowErrors)
            {
                throw e;
            }

            return false;
        }
        catch (Exception e)
        {
            Debug.LogError("Error when attempting to deserialize data from " + fullpath + ". Reason: " + e.Message);

            if (fs != null)
            {
                fs.Close();
            }

            try
            {
                fs.Dispose();
            }
            catch (Exception e2)
            {
                Debug.LogWarning(e2.Message);
            }

            data = default(object);

            if (shouldRethrowErrors)
            {
                throw e;
            }

            return false;
        }

        if (DebugManager.instance.enableDebug && DebugManager.instance.debugSerializationSuccessMessages)
        {
            Debug.Log("Retrieved data successfully from " + fullpath);
        }

        if (fs != null)
        {
            fs.Close();
        }

        try
        {
            fs.Dispose();
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }

        return true;
    }


   

}
