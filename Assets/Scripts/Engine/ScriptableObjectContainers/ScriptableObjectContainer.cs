using EditorAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.Linq;

namespace Engine
{
    public abstract class ScriptableObjectContainer<T> : ScriptableObjectContainerBase where T : ContaineableScriptableObject
    {
        [SerializeField] protected List<T> Contained = new();
        public virtual List<T> All => Contained;
        public override Type ChildClassesBaseType => typeof(T);
        public override int Count => Contained.Count;
        public override void Add(ContaineableScriptableObject toAdd)
        {
            if (!Contained.Contains((T)toAdd))
                Contained.Add((T)toAdd);
        }

        public T FindWithID(long id)
        {
            return Contained.FirstOrDefault(x => x.AssetGUID == id);
        }

#if UNITY_EDITOR
        public T CreateNewInstance() => CreateNewInstanceOfType<T>();
        [Button]
        private void FixContainedObjects()
        {
            Contained.Clear();
            foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this.GetInstanceID())))
            {
                var converted = asset as T;
                if (converted != null)
                    Add(converted);

            }
            AssetDatabase.SaveAssets();
        }
#endif
        public override void Destroy(ContaineableScriptableObject toDestroy)
        {
            Contained.Remove((T)toDestroy);
            DestroyImmediate(toDestroy, true);
        }

        public override bool ContainsElementOfType(Type type)
        {
            foreach (var item in Contained)
            {
                if (item.GetType() == type)
                    return true;
            }

            return false;
        }


        protected virtual bool GenerateEnum => false;

        protected virtual Type GetGeneratedEnumContainerType()
        {
            return null;
        }
        protected virtual string GetGeneratedEnumName()
        {
            return nameof(T) + "ID";
        }

        protected virtual string GetGeneratedEntryName(T _t)
        {
            var name = _t.name;

            if (!char.IsLetter(name[0]))
                name = "_" + name;

            name = name.Replace("-", "_");
            return name;
        }


#if UNITY_EDITOR

        private TextAsset GetGeneratedEnumFile()
        {
            var type = GetGeneratedEnumContainerType();
            if (type == null)
                return null;

            var asset = EditorUtil.FindProjectScriptAssetForType(type);
            return asset;
        }

        [Button(nameof(GenerateEnum), ConditionResult.ShowHide)]
        private void UpdateGeneratedEnum()
        {
            var file = GetGeneratedEnumFile();
            var enumName = GetGeneratedEnumName();

            if (file == null || enumName == null)
            {
                return;
            }

            const string startRegion = "#region GENERATED (ScriptableObjectContainer)";
            const string endRegion = "#endregion GENERATED (ScriptableObjectContainer)";
            var text = file.text;
            int start = text.IndexOf(startRegion) + startRegion.Length;
            int end = text.IndexOf(endRegion);

            bool creatingGeneratedRegion = false;

            StringBuilder sb = new StringBuilder();
            if (start == -1 || end == -1)
            {
                creatingGeneratedRegion = true;
                start = text.LastIndexOf('}');
            }

            sb.Append(text.Substring(0, start));
            if (creatingGeneratedRegion)
                sb.AppendLine(startRegion);

            sb.AppendLine("");
            sb.AppendLine($"    public enum {enumName} : long");
            sb.AppendLine("    {");

            foreach (var item in Contained)
            {
                var name = GetGeneratedEntryName(item);
                string line = $"        {name} = {item.AssetGUID},";
                sb.AppendLine(line);
            }

            sb.AppendLine("    }");

            if (creatingGeneratedRegion)
            {
                sb.AppendLine(endRegion);
                sb.AppendLine("}");
            }
            else
                sb.Append(text.Substring(end));


            var localPath = AssetDatabase.GetAssetPath(file);
            //Application.dataPath - "Assets"
            var path = Application.dataPath.Substring(0, Application.dataPath.Length - 6)
                + localPath;

            System.IO.File.WriteAllText(path, sb.ToString());
            AssetDatabase.ImportAsset(localPath);
        }

#endif
    }
}
