using System.Collections.Generic;
using FriendsGamesTools.ECSGame;
using FriendsGamesTools.Analytics;
using UnityEngine;
using System.Linq;
using System.Text;
using FriendsGamesTools.CodeGeneration;

namespace FriendsGamesTools.ABTests
{
    public class ABTestModule : ECSModule
    {
        public const string define = "ECS_AB_TEST";
        public override string Define => define;
        public override HowToModule HowTo() => new ECS_AB_TEST_HowTo();
        public override List<string> dependFromModules => base.dependFromModules.Adding(AnalyticsModule.define);
        public const string RunCodegeneration = "Run codegeneration";

#if ECS_AB_TEST
        protected override string debugViewPath => $"ECSGame/ECS_AB_TEST/ABTestDebugPanel";
        bool inited;
        void InitIfNeeded()
        {
            if (inited) return;
            inited = true;
            UpdateCodeGen();
        }
        protected override void OnCompiledGUI()
        {
            base.OnCompiledGUI();
            var changed = false;
            EditorGUIUtils.Toggle("show on main page", ref config.showOnMainWindow, ref changed);
            if (changed)
                EditorUtils.SetDirty(config);
            ShowConfigOnGUI();
        }
        public override void ShortcutOnGUI()
        {
            if (config.showOnMainWindow)
                ShowConfigOnGUI();
        }

        ABTestsConfig config => SettingsInEditor<ABTestsConfig>.instance;
        bool editing;
        StringBuilder sb = new StringBuilder();
        void ShowConfigOnGUI()
        {
            InitIfNeeded();
            var changed = false;
            GUILayout.BeginHorizontal();
            sb.Clear();
            EditorGUIUtils.ShowValid(IsValid(config, sb));
            EditorGUIUtils.RichMultilineLabel(config.tests.Count == 0 ? "no AB tests exists" : $"Existing AB tests(<b>{config.tests.Count}</b>):");
            if (!editing || FriendsGamesToolsWindow.instance.shownParenModule != this)
            {
                if (GUILayout.Button("Edit AB tests"))
                {
                    FriendsGamesToolsWindow.instance.OnShowModulePressed(this);
                    editing = true;
                }
            } else
            {
                if (GUILayout.Button("finish editing"))
                {
                    editing = false;
                    changed = true;
                }
            }
            GUILayout.EndHorizontal();

            if (editing)
            {
                if (GUILayout.Button("Add test"))
                {
                    config.tests.Add(new TestConfig
                    {
                        name = "NewTest",
                        options = new List<TestOptionConfig> {
                            new TestOptionConfig { optionSuffix = "A" },
                            new TestOptionConfig { optionSuffix = "B"}
                        }
                    });
                }
            }

            for (int i = 0; i < config.tests.Count; i++)
            {
                const string nameTitle = "AB test name: ";
                var currTest = config.tests[i];
                GUILayout.BeginHorizontal();
                EditorGUIUtils.Indent();
                EditorGUIUtils.ShowValid(IsValidTest(currTest, false));
                if (editing)
                {
                    EditorGUIUtils.TextField(nameTitle, ref currTest.name, ref changed);
                    var enabled = currTest.enabled;
                    if (EditorGUIUtils.Toggle("enabled", ref enabled, ref changed))
                    {
                        if (enabled)
                            currTest.disabledAlwaysEventInd = -1;
                        else
                            currTest.disabledAlwaysEventInd = 0;
                    }
                    
                    if (EditorGUIUtils.XButton())
                    {
                        config.tests.RemoveAt(i);
                        i--;
                        changed = true;
                    }
                }
                else
                {
                    EditorGUIUtils.RichMultilineLabel($"{nameTitle} <b>{currTest.name}</b>");
                    EditorGUIUtils.PushDisabling();
                    GUILayout.Toggle(currTest.enabled, "enabled");
                    if (!currTest.enabled)
                        EditorGUIUtils.RichMultilineLabel($"always <b>{currTest.options[currTest.disabledAlwaysEventInd].optionSuffix}</b>");
                    EditorGUIUtils.PopEnabling();
                }
                GUILayout.EndHorizontal();

                // Options.
                if (editing)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUIUtils.Indent(2);
                    if (GUILayout.Button("Add option"))
                        currTest.options.Add(new TestOptionConfig { optionSuffix = "NewOption" });
                    GUILayout.EndHorizontal();
                }
                for (int j = 0; j < currTest.options.Count; j++)
                {
                    var currOption = currTest.options[j];
                    GUILayout.BeginHorizontal();
                    EditorGUIUtils.Indent(2);
                    EditorGUIUtils.ShowValid(IsValidOption(j, currTest));
                    if (editing)
                    {
                        GUILayout.Label(currTest.name, GUILayout.Width(93));
                        EditorGUIUtils.TextField("", ref currOption.optionSuffix, ref changed, 100);
                        EditorGUIUtils.Indent(14);
                        EditorGUIUtils.IntField("mass", ref currOption.optionMass, ref changed);
                    } else
                    {
                        EditorGUIUtils.RichMultilineLabel($"<b>{currTest.GetOptionName(j)}</b>");
                    }
                    EditorGUIUtils.RichMultilineLabel($"{(currOption.optionMass/(float)currTest.options.ConvertAll(o=>o.optionMass).Sum()).ToShownPercents()}");
                    if (editing && EditorGUIUtils.XButton())
                    {
                        currTest.options.RemoveAt(j);
                        j--;
                        changed = true;
                    }
                    GUILayout.EndHorizontal();
                }
            }
            if (sb.Length > 0)
                EditorGUIUtils.Error(sb.ToString());

            ShowCodeGeneration();

            if (changed)
            {
                EditorUtils.SetDirty(config);
            }
        }

        bool IsValidOption(int optionInd, TestConfig test, StringBuilder sb = null)
        {
            var valid = true;
            var option = test.options[optionInd];
            var name = test.GetOptionName(optionInd);
            if (!ReflectionUtils.CanBeIdentifier(name))
            {
                sb?.AppendLine($"option name '{name}' is not valid identifier");
                valid = false;
            }
            if (option.optionMass < 0)
            {
                sb?.AppendLine("mass cant be negative");
                valid = false;
            }
            return valid;
        }
        bool IsValidTest(TestConfig test, bool checkOptions, StringBuilder sb = null)
        {
            var valid = true;
            if (!test.enabled)
                return true;
            if (!ReflectionUtils.CanBeIdentifier(test.name))
            {
                sb?.AppendLine($"test name '{test.name}' is not valid identifier");
                valid = false;
            }
            Utils.ForeachDuplicate(test.options, option => option.optionSuffix, (i, j, duplicate) =>
            {
                sb?.AppendLine($"test {test.name} has duplicate suffixes {test.options[j].optionSuffix} in {i}-th and {j}-th option");
                valid = false;
            });
            if (test.options.Count < 2)
            {
                sb?.AppendLine($"test {test.name} have less than 2 options");
                valid = false;
            }
            if (checkOptions)
            {
                for (int i = 0; i < test.options.Count; i++)
                {
                    if (!IsValidOption(i, test, sb))
                        valid = false;
                }
            }
            return valid;
        }
        bool IsValid(ABTestsConfig config, StringBuilder sb = null)
        {
            var valid = true;
            config.tests.ForEach(t=> {
                if (!IsValidTest(t, true, sb))
                    valid = false;
            });
            Utils.ForeachDuplicate(config.tests, test => test.name, (i, j, duplicate) =>
            {
                sb?.AppendLine($"duplicate {duplicate} name in {i}-th and {j}-th test");
                valid = false;
            });
            if (!CodegenValid(sb))
                valid = false;
            return valid;
        }
        public override string DoReleaseChecks()
        {
            sb.Clear();
            UpdateCodeGen();
            IsValid(config, sb);
            return sb.ToString();
        }

        #region Code generation
        CodeGenerator codegen = new CodeGenerator();
        void UpdateCodeGen()
        {
            var nameSpaceName = "FriendsGamesTools.ABTests";
            var folder = codegen.RequireFolder("ABTests");
            var fileCS = folder.RequireFile("ABTestsController_Generated");
            var nameSpace = fileCS.RequireNameSpace(nameSpaceName);
            var ABTestsController = nameSpace.RequireClass("ABTestsController");
            ABTestsController.visibility.RequirePublic().partialization.RequirePartial();
            var generatedPropertiesCount = 0;
            config.tests.ForEachWithInd((t,testInd)=>t.options.ForEachWithInd((o, optionInd)=> {
                ABTestsController.RequireProperty("bool", t.GetOptionName(o))
                    .staticity.RequireStatic().visibility.RequirePublic().RequireExpressionBody($"instance.data[{testInd}].value == {optionInd}");
                generatedPropertiesCount++;
            }));
            fileCS.RequireCodeEntriesCount("public static bool ", generatedPropertiesCount);
            codegenComplete = codegen.complete;
            codegenHash = CalcCodegenHash();
        }
        bool codegenComplete;
        const string CodegenNotCompleted = "AB tests Codegen not completed";
        bool CodegenValid(StringBuilder sb = null)
        {
            if (!codegenComplete)
            {
                sb?.AppendLine(CodegenNotCompleted);
                return false;
            }
            return true;
        }
        long codegenHash;
        long CalcCodegenHash()
        {
            long hash = 1;
            config.tests.ForEach(t =>
            {
                hash = hash.ToHash(t.name.ToHash());
                t.options.ForEach(o =>
                {
                    hash = hash.ToHash(o.optionSuffix.ToHash());
                });
            });
            return hash;
        }
        void UpdateCodegenStatus()
        {
            if (codegenComplete && codegenHash != CalcCodegenHash())
                codegenComplete = false;
        }
        void ShowCodeGeneration()
        {
            UpdateCodegenStatus();
            EditorGUIUtils.InHorizontal(() => {
                sb.Clear();
                EditorGUIUtils.ShowValid(codegenComplete);
                if (!codegenComplete)
                    EditorGUIUtils.Error(CodegenNotCompleted);
                else
                    GUILayout.Label("Code generation complete");
            });
            if (codegenComplete)
                return;
            if (GUILayout.Button(RunCodegeneration))
                codegen.Generate();
        }
        #endregion
#endif
    }
}


