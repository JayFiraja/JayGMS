using System.IO;

namespace GMS.Editor
{
    /// <summary>
    /// Class with the logic for writing submanager files.
    /// </summary>
    public static class SubManagerWriter 
    {
        
        public static void CreateLogicClass(string logicClassPath, bool useCustomNamespace, string defaultNamespace, string nameSpace, string dataStructName, string logicClassName)
        {
            using (StreamWriter writer = new StreamWriter(logicClassPath, false))
            {
                if (useCustomNamespace)
                {
                    writer.WriteLine($"using {defaultNamespace};");
                    writer.WriteLine();
                }
                string namespaceToUse = useCustomNamespace ? nameSpace : defaultNamespace;
                writer.WriteLine($"namespace {namespaceToUse}");
                writer.WriteLine("{");
                writer.WriteLine("    public class " + logicClassName + ": ISubManager");
                writer.WriteLine("    {");
                writer.WriteLine();
                writer.WriteLine("        private " + dataStructName + " _data;"); // data Field
                writer.WriteLine();
                writer.WriteLine("        public " + logicClassName + "(" + dataStructName + " data)"); // Constructor that will get the data (dependency injection)
                writer.WriteLine("        {");
                writer.WriteLine("            _data = data;");
                writer.WriteLine("        }");

                writer.WriteLine();
                
                // implement interface ISubManager
                string addDefaultNameSpace = useCustomNamespace ? $"{defaultNamespace}." : string.Empty;
                writer.WriteLine($"        public bool Initialize({addDefaultNameSpace}GameManager gameManager)");
                writer.WriteLine("        {");
                writer.WriteLine("            // implement initialization here");
                writer.WriteLine("            return true;");
                writer.WriteLine("        }");
                writer.WriteLine();
                writer.WriteLine("        public void UnInitialize()");
                writer.WriteLine("        {");
                writer.WriteLine("            // Cleanup, ie. unload instances, send cleanup messages");
                writer.WriteLine("        }");
                writer.WriteLine();
                writer.WriteLine("        public void OnUpdate()");
                writer.WriteLine("        {");
                writer.WriteLine("            // Ticked by GameManager every frame. Implement logic that requires constant update here");
                writer.WriteLine("        }");
                writer.WriteLine();
                writer.WriteLine("        public bool Equals(ISubManager other)");
                writer.WriteLine("        {");
                writer.WriteLine("            if (other == null) return false;");
                writer.WriteLine();
                writer.WriteLine("            // Compare the runtime types of the current instance and the other instance");
                writer.WriteLine("            return GetType() == other.GetType();");
                writer.WriteLine("        }");
                writer.WriteLine();
                writer.WriteLine("        public override bool Equals(object obj)");
                writer.WriteLine("        {");
                writer.WriteLine("            if (obj is ISubManager other)");
                writer.WriteLine("            {");
                writer.WriteLine("                return Equals(other);");
                writer.WriteLine("            }");
                writer.WriteLine("            return false;");
                writer.WriteLine("        }");
                writer.WriteLine();
                writer.WriteLine("        public override int GetHashCode()");
                writer.WriteLine("        {");
                writer.WriteLine("            // Generate a hash code based on the fields that contribute to equality");
                writer.WriteLine("            return GetType().GetHashCode();");
                writer.WriteLine("        }");
                writer.WriteLine("    }");
                writer.WriteLine("}");
            }
        }

        public static void CreateDataStruct(string dataStructPath, bool useCustomNamespace, string defaultNamespace, string nameSpace, string dataStructName, string logicClassName)
        {
            using (StreamWriter writer = new StreamWriter(dataStructPath, false))
            {
                if (useCustomNamespace)
                {
                    writer.WriteLine($"using {defaultNamespace};");
                }
                string namespaceToUse = useCustomNamespace ? nameSpace : defaultNamespace;
                writer.WriteLine($"namespace {namespaceToUse}");
                writer.WriteLine("{");
                writer.WriteLine("    [System.Serializable]");
                writer.WriteLine($"    [LinkDataLogic(typeof({dataStructName}), dataDisplayName: \"{dataStructName}\", typeof({logicClassName}), displayName: \"{logicClassName}\")]");
                writer.WriteLine($"    public struct {dataStructName} : ISubManagerData");
                writer.WriteLine("    {");
                writer.WriteLine("        // Add Data fields");
                writer.WriteLine("    }");
                writer.WriteLine("}");
            }
        }
    }
}

