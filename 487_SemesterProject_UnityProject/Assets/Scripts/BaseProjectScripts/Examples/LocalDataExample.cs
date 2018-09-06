using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization;

namespace Examples
{
    [System.Serializable]
    public class LocalDataExample : MonoBehaviour
    {
        [Header("Reference type matches object instance type")]
        public TestClassBase base_As_Base = new TestClassBase();
        public DerivedClassA derivedA_As_DerivedA = new DerivedClassA();
        public DerivedClassB derivedB_As_DerivedB = new DerivedClassB();

        [Header("Reference type does not match object instance type")]
        public TestClassBase derivedA_As_Base = new DerivedClassA();
        public TestClassBase derivedB_As_Base = new DerivedClassB();

        [Header("References")]
        [SerializeField] Graphic jsonPrettyToggleGraphic;

        private void Start()
        {
            DebugManager.instance.enableDebug = true;
            DebugManager.instance.debugSerializationSuccessMessages = true;
        }
        

        public void LoadBinary()
        {
            Debug.LogWarning("LoadBinary()");
            LocalDataManager localDataManager = LocalDataManager.instance;

            localDataManager.LoadObjectFromFile<TestClassBase>("LocalDataTutorial/binary", "base.object-binary", out base_As_Base, LocalDataManager.SerializationType.binary);
            localDataManager.LoadObjectFromFile<DerivedClassA>("LocalDataTutorial/binary", "derivedA.object-binary", out derivedA_As_DerivedA, LocalDataManager.SerializationType.binary);
            localDataManager.LoadObjectFromFile<DerivedClassB>("LocalDataTutorial/binary", "derivedB.object-binary", out derivedB_As_DerivedB, LocalDataManager.SerializationType.binary);
            localDataManager.LoadObjectFromFile<TestClassBase>("LocalDataTutorial/binary", "derivedA.object-binary", out derivedA_As_Base, LocalDataManager.SerializationType.binary);
            localDataManager.LoadObjectFromFile<TestClassBase>("LocalDataTutorial/binary", "derivedB.object-binary", out derivedB_As_Base, LocalDataManager.SerializationType.binary);
        }

        public void SaveBinary()
        {
            Debug.LogWarning("SaveBinary()");
            LocalDataManager localDataManager = LocalDataManager.instance;

            localDataManager.SaveObjectToFile("LocalDataTutorial/binary", "base.object-binary", base_As_Base, LocalDataManager.SerializationType.binary);
            localDataManager.SaveObjectToFile("LocalDataTutorial/binary", "derivedA.object-binary", derivedA_As_DerivedA, LocalDataManager.SerializationType.binary);
            localDataManager.SaveObjectToFile("LocalDataTutorial/binary", "derivedB.object-binary", derivedB_As_DerivedB, LocalDataManager.SerializationType.binary);
        }


        public void LoadXml()
        {
            Debug.LogWarning("LoadXml()");
            LocalDataManager localDataManager = LocalDataManager.instance;

            localDataManager.LoadObjectFromFile<TestClassBase>("LocalDataTutorial/xml", "base.xml", out base_As_Base, LocalDataManager.SerializationType.xml);
            localDataManager.LoadObjectFromFile<DerivedClassA>("LocalDataTutorial/xml", "derivedA.xml", out derivedA_As_DerivedA, LocalDataManager.SerializationType.xml);
            localDataManager.LoadObjectFromFile<DerivedClassB>("LocalDataTutorial/xml", "derivedB.xml", out derivedB_As_DerivedB, LocalDataManager.SerializationType.xml);
            localDataManager.LoadObjectFromFile<TestClassBase>("LocalDataTutorial/xml", "derivedA.xml", out derivedA_As_Base, LocalDataManager.SerializationType.xml);
            localDataManager.LoadObjectFromFile<TestClassBase>("LocalDataTutorial/xml", "derivedB.xml", out derivedB_As_Base, LocalDataManager.SerializationType.xml);
        }

        public void SaveXml()
        {
            Debug.LogWarning("SaveXml()");
            LocalDataManager localDataManager = LocalDataManager.instance;

            localDataManager.SaveObjectToFile("LocalDataTutorial/xml", "base.xml", base_As_Base, LocalDataManager.SerializationType.xml);
            localDataManager.SaveObjectToFile("LocalDataTutorial/xml", "derivedA.xml", derivedA_As_DerivedA, LocalDataManager.SerializationType.xml);
            localDataManager.SaveObjectToFile("LocalDataTutorial/xml", "derivedB.xml", derivedB_As_DerivedB, LocalDataManager.SerializationType.xml);
        }

        //public void LoadXmlContract()
        //{
        //    Debug.LogWarning("LoadXmlContract()");
        //    LocalDataManager localDataManager = LocalDataManager.instance;
        //
        //    localDataManager.LoadObjectFromFile<TestClassBase>("base_Contract.xml", out base_As_Base, LocalDataManager.SerializationType.xmlDataContract);
        //    localDataManager.LoadObjectFromFile<DerivedClassA>("derivedA_Contract.xml", out derivedA_As_DerivedA, LocalDataManager.SerializationType.xmlDataContract);
        //    localDataManager.LoadObjectFromFile<DerivedClassB>("derivedB_Contract.xml", out derivedB_As_DerivedB, LocalDataManager.SerializationType.xmlDataContract);
        //    localDataManager.LoadObjectFromFile<TestClassBase>("derivedA_Contract.xml", out derivedA_As_Base, LocalDataManager.SerializationType.xmlDataContract);
        //    localDataManager.LoadObjectFromFile<TestClassBase>("derivedB_Contract.xml", out derivedB_As_Base, LocalDataManager.SerializationType.xmlDataContract);
        //}

        public void SaveXmlContract()
        {
            Debug.LogWarning("SaveXmlContract()");
            LocalDataManager localDataManager = LocalDataManager.instance;

            localDataManager.SaveObjectToFile("base_Contract.xml", base_As_Base, LocalDataManager.SerializationType.xmlDataContract);
            localDataManager.SaveObjectToFile("derivedA_Contract.xml", derivedA_As_DerivedA, LocalDataManager.SerializationType.xmlDataContract);
            localDataManager.SaveObjectToFile("derivedB_Contract.xml", derivedB_As_DerivedB, LocalDataManager.SerializationType.xmlDataContract);
        }

        public void LoadJson()
        {
            Debug.LogWarning("LoadJson()");
            LocalDataManager localDataManager = LocalDataManager.instance;

            localDataManager.LoadObjectFromFile<TestClassBase>("LocalDataTutorial/json", "base.json", out base_As_Base, LocalDataManager.SerializationType.json);
            localDataManager.LoadObjectFromFile<DerivedClassA>("LocalDataTutorial/json", "derivedA.json", out derivedA_As_DerivedA, LocalDataManager.SerializationType.json);
            localDataManager.LoadObjectFromFile<DerivedClassB>("LocalDataTutorial/json", "derivedB.json", out derivedB_As_DerivedB, LocalDataManager.SerializationType.json);
            localDataManager.LoadObjectFromFile<TestClassBase>("LocalDataTutorial/json", "derivedA.json", out derivedA_As_Base, LocalDataManager.SerializationType.json);
            localDataManager.LoadObjectFromFile<TestClassBase>("LocalDataTutorial/json", "derivedB.json", out derivedB_As_Base, LocalDataManager.SerializationType.json);
        }

        public void SaveJson()
        {
            Debug.LogWarning("SaveJson()");
            LocalDataManager localDataManager = LocalDataManager.instance;

            localDataManager.SaveObjectToFile("LocalDataTutorial/json", "base.json", base_As_Base, LocalDataManager.SerializationType.json);
            localDataManager.SaveObjectToFile("LocalDataTutorial/json", "derivedA.json", derivedA_As_DerivedA, LocalDataManager.SerializationType.json);
            localDataManager.SaveObjectToFile("LocalDataTutorial/json", "derivedB.json", derivedB_As_DerivedB, LocalDataManager.SerializationType.json);
        }

        public void OnJsonPrettyToggled()
        {
            Debug.LogWarning("OnJsonPrettyToggled()");
            LocalDataManager.instance.useJsonPretty = !LocalDataManager.instance.useJsonPretty;
            jsonPrettyToggleGraphic.gameObject.SetActive(!jsonPrettyToggleGraphic.gameObject.activeInHierarchy);
            Debug.Log("Set \"use json pretty\" to " + LocalDataManager.instance.useJsonPretty);
        }

        public void PrintTypes()
        {
            Debug.LogWarning("PrintTypes()");
            Debug.Log("Base object using Base reference is a " + base_As_Base.GetType().ToString());
            Debug.Log("DerivedClassA object using DerivedClassA reference is a " + derivedA_As_DerivedA.GetType().ToString());
            Debug.Log("DerivedClassB object using DerivedClassB reference is a " + derivedB_As_DerivedB.GetType().ToString());
            Debug.Log("DerivedClassA object using Base reference is a " + derivedA_As_Base.GetType().ToString());
            Debug.Log("DerivedClassB object using Base reference is a " + derivedB_As_Base.GetType().ToString());
        }
    }

    [DataContract]
    [System.Serializable]
    public class TestClassBase
    {
        [DataMember] public string label = "default label";
        [DataMember] public int id = 0;
    }

    [DataContract]
    [System.Serializable]
    public class DerivedClassA : TestClassBase
    {
        [DataMember] public float floatingPointNumber = 39.39f;
    }

    [DataContract]
    [System.Serializable]
    public class DerivedClassB : TestClassBase
    {
        [DataMember] public bool testBool = true;
    }
}
