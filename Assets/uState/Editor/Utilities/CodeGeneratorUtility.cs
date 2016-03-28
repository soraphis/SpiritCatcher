/**
Copyright (c) <2015>, <Devon Klompmaker>
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the <organization> nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
**/
using UnityEngine;
using UnityEditor;
using UnityEditor.Scripting.Compilers;
using System;
using System.Collections.Generic;
using System.IO;

namespace uState
{
    public class CodeGeneratorUtility
    {
        public static void GeneratePropertiesFromModel(Model model)
        {
            string safeModelName = model.name.Replace(" ", string.Empty);
            string className = safeModelName + "Properties";
            string basePath = ProjectUtilities.GetRootPathOf(model);
            string writePath = basePath + className + ".cs";

            List<string> fields = new List<string>();

            foreach (Property property in model.properties)
            {
                string fieldText = "\tpublic static string ";

                string safeName = property.name.Replace(" ", string.Empty);

                fieldText += safeName + " = \"" + property.name + "\"";

                fieldText += ";\r\n";

                fields.Add(fieldText);
            }

            TextAsset interfaceAsset = Resources.Load<TextAsset>("Templates/ModelPropertiesTemplate");

            string interfaceTemplate = interfaceAsset.text;
            interfaceTemplate = interfaceTemplate.Replace("{CLASS_NAME}", className);
            interfaceTemplate = interfaceTemplate.Replace("{CLASS_BODY}", string.Join(string.Empty, fields.ToArray()));

            if (!string.IsNullOrEmpty(model.propertiesAssetGUID))
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(model.propertiesAssetGUID);

                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset));

                if (obj != null && obj.name != className)
                {
                    basePath = ProjectUtilities.GetRootPathOf(obj);
                    writePath = basePath + className + ".cs";

                    AssetDatabase.DeleteAsset(assetPath);
                }
                else
                {
                    writePath = assetPath;
                }
            }

            File.WriteAllText(writePath, interfaceTemplate);

            AssetDatabase.ImportAsset(writePath);
            AssetDatabase.Refresh();

            model.propertiesAssetGUID = AssetDatabase.AssetPathToGUID(writePath);

        }

        public static void GenerateInterfaceFromModel(Model model)
        {
            foreach (State state in model.states)
            {
                List<string> stateNames = new List<string>();

                string safeStateName = state.name.Replace(" ", string.Empty);
                string className = "I" + model.name + safeStateName + "Handler";

                if (state.generateEnter)
                {
                    stateNames.Add("\tvoid OnEnter" + safeStateName + "();\r\n");
                }

                if (state.generateUpdate)
                {
                    stateNames.Add("\tvoid On" + safeStateName + "();\r\n");
                }

                if (state.generateExit)
                {
                    stateNames.Add("\tvoid OnExit" + safeStateName + "();\r\n");
                }

                if(stateNames.Count == 0)
                {
                    if(!string.IsNullOrEmpty(state.interfaceAssetGUID))
                    {
                        AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(state.interfaceAssetGUID));
                    }

                    continue;
                }

                TextAsset interfaceAsset = Resources.Load<TextAsset>("Templates/InterfaceTemplate");

                string interfaceTemplate = interfaceAsset.text;
                interfaceTemplate = interfaceTemplate.Replace("{INTERFACE_NAME}", className);
                interfaceTemplate = interfaceTemplate.Replace("{INTERFACE_BODY}", string.Join(string.Empty, stateNames.ToArray()));

                string basePath = ProjectUtilities.GetRootPathOf(model);
                string interfacePath = basePath + className + ".cs";

                state.interfaceName = className;
                state.safeStateName = safeStateName;

                bool skipImport = false;

                if (!string.IsNullOrEmpty(state.interfaceAssetGUID))
                {
                    string tempPath = AssetDatabase.GUIDToAssetPath(state.interfaceAssetGUID);
                    UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(tempPath, typeof(TextAsset));

                    if (obj != null)
                    {
                        if (obj.name != className)
                        {
                            basePath = ProjectUtilities.GetRootPathOf(obj);
                            string newPath = basePath + className + ".cs";
                            interfacePath = newPath;

                            AssetDatabase.DeleteAsset(tempPath);

                            File.WriteAllText(newPath, interfaceTemplate);
                        }
                        else
                        {
                            interfacePath = tempPath;
                            skipImport = true;

                            File.WriteAllText(tempPath, interfaceTemplate);
                        }
                    }
                    else
                    {
                        File.WriteAllText(interfacePath, interfaceTemplate);
                    }
                }
                else
                {
                    File.WriteAllText(interfacePath, interfaceTemplate);
                }

                if (!skipImport)
                {
                    AssetDatabase.ImportAsset(interfacePath);

                    state.interfaceAssetGUID = AssetDatabase.AssetPathToGUID(interfacePath);
                }
            }

            AssetDatabase.Refresh();
        }

        public static Model GenerateModel(string name)
        {
            Model model = ProjectUtilities.CreateAsset<Model>(name + "Model");
            return model;
        }
    }

}