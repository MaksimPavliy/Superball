using UnityEngine;

namespace FriendsGamesTools
{
    public class UtilsModule_HowTo : HowToModule
    {
        public override string forWhat => "lots of small often useful utils, mostly extensions";
        protected override void OnHowToGUI()
        {
            EditorGUIUtils.LabelAtCenter("FOR EDITOR", FontStyle.Bold);

            DefinesModifier.ShowOnGUI("Modifies defines",
                "Methods like DefinesModifier.<b>AddDefine(define)</b>, <b>RemoveDefine(define)</b> " +
                "<b>ReplaceDefine(toRemove, toAdd)</b> <b>DefineExists(define)</b>");

            EditorUtils.ShowOnGUI("Utils specially for editor",
                "Utils for project assets managing:\n" +
                "<b>SetDirty ClearFolder CreateScriptableInstanceAtPath GetAllInstancesInProject GetInstanceInProject</b>\n" +
                "Utils for private members accessing through reflection:\n" +
                "<b>GetPrivateFieldInfo GetPrivateField SetPrivateField CallPrivateMethod</b>");

            CompilationCallback.ShowOnGUI("Call <b>CompilationCallback.CallStaticMethodAfterRecompilation(StaticMethod)</b> to do smth after scripts recompiled.",
                "Useful for advanced plugins");

            GUIUtils.ShowOnGUI("Methods for writing custom gui",
                "<b>ColoredLabel(text, col), RichLabel(...), TextField(label, ref val, ref changed),\n" +
                "RichMultilineLabel(textWithRichTags), URL(url), Toolbar(TEnum val)</b>");

            ReflectionUtils.ShowOnGUI("Automation based on reflection",
                "Knows about all executing and editor unity assemblies\n" +
                "<b>ForEachDerivedType<T>(type=>{})</b>, <b>GetAllDerivedTypes(type)</b> - iterate all types derived from T\n" +
                "<b>DoesClassExistInUnityAssembly(classNameWithNamespace)</b> - checks if class exists\n" +
                "<b>memberInfo.GetMemberType()</b> - gets underlying FieldInfo or PropertyInfo type\n" +
                "<b>type.GetMembersWithAttribute<TAttr>(memberInfo=>{})</b> - iterate all field or property infos with certain attribute");

            EditorGUIUtils.LabelAtCenter("Make unity features more convenient", FontStyle.Bold);

            SettingsScriptableCreator.ShowOnGUI("Makes using scriptable objects easy",
                "Derive your settings from SettingsScriptable and \n" +
                "call SettingsInEditor<TSettings>.<b>EnsureExists()</b> to create settings if they dont exist,\n" +
                "SettingsInEditor<TSettings>.<b>GetSettingsInstance(createIfNotExists)</b> to get instance.\n" +
                "You can automatically create instance if needed.\n" +
                "Also any ScriptableObject-derived script has <b>button to create its instance</b> in project\n" +
                " - select script file, press little gear in inspector (lower one, there's 2 gears there) and you'll see a button");

            SettingsScriptable.ShowOnGUI("Derive from this script to have conveniently managable scriptable object",
                "It will single <b>instance</b> in project, just like a scriptable singleton\n" +
                "Instance will be located in <b>FriendsGamesGenerated</b> folder, but you can override <b>SubFolder</b> if needed\n" +
                "override <b>inResources</b> if you need to be able to load it in runtime");

            DestroyOnAwake.ShowOnGUI("Put it on game objects that should exist only in editor");

            MonoBehaviourHasInstance.ShowOnGUI("Like singleton, but simpler",
                "Just saves its <b>instance</b> in Awake");

            PlayerPrefsUtils.ShowOnGUI("player prefs extensions",
                "<b>SetDouble</b> and <b>GetDouble</b> methods");

            SerializationUtils.ShowOnGUI("Serialize anything with <b>[Serializable]</b> tag to byte array or string",
                "<b>SerializeToByteArray(t)</b> / <b>Deserialize<T>(bytes)</b>");

            StringUtils.ShowOnGUI("string utils",
                "Check digit in string - <b>s.DigitsCount()</b>, <b>s.HasDigits()</b>\n" +
                "Line endings - <b>s.WithLineEndings(\"\"n\")</b>, <b>s.ToCrLf()</b>, <b>s.ToLf()</b>\n" +
                "Clipboard - <b>s.CopyToClipboard()</b>, <b>s.PasteFromClipboard()</b>\n" +
                "Parsing regardless of , or . used - <b>TryParse(s, out val)</b>");

            TransformUtils.ShowOnGUI("Transform utils",
                "<b>tr.SetDefaultLocalPosition()</b> - sets local matrix to identity\n" +
                "<b>tr.GetOrAddComponent<T>()</b>\n" +
                "<b>tr.GetInterfacesInChildren()</b> - gets ingterface-implementing monobehaviours\n" +
                "<b>tr.IterateChildren(childTr=>{})</b> - recursive child transform iteration\n" +
                "<b>tr.SetLayerRecursively(layer)</b>\n" +
                "<b>tr.DestroyChildren()</b>");

            EditorGUIUtils.LabelAtCenter("View utils", FontStyle.Bold);

            AnimUtils.ShowOnGUI("Utils for sprite animation",
                "(FPS, loopsCount) = <b>FitDurationFPSFitDurationFPS</b>(\n" +
                "   duration, notLoopedFramesCount, loopedFramesCount, idealFPS)\n" +
                "Idea is to make some process with configured length\n" +
                "consisting from looped and not looped anim clips,\n" +
                "by tweaking loop count and animation speed\n" +
                "in order to minimize animation speed changes from ideal anim speed.\n" +
                "(speed, loopsCount) = <b>FitDuration</b>(duration, notLoopedDuration, loopedDuration, idealSpeed)\n" +
                "same as previous, only speed instead of FPS");

            AsyncUtils.ShowOnGUI("Utils for async programming (using AsyncAwaitUtil plugin)",
                "await AsyncUtils.<b>SecondsWithProgress</b>(duration, progress =>{}) - convenienty implement some async process");

            PrefabUtils.ShowOnGUI("Utils for prefabs",
                "<b>IsPrefabOpened()</b> - true when prefab stage shown\n" +
                "<b>go.IsPrefab()</b>\n" +
                "<b>UpdatePrefabsList</b>(views, data, prefab, parent, updateView)\n" +
                "   - support list of views where each view is an instance of prefab (instantiated under parent)\n" +
                "   and updated view updateView. Not needed prefabs are disabled, so it's like simple pool\n" +
                "<b>UpdatePrefabsDictionary</b>(views, data, getPrefab, parent, updateView)\n" +
                "   - same as previous, but data is in dictionary, so are views.\n" +
                "   getPrefab selects prefab for each data item. No pool though");

            EditorGUIUtils.LabelAtCenter("FGT formats", FontStyle.Bold);

            ExpFormula.ShowOnGUI("Canonical exponential formula",
                "<b>Default + Base * Coef^power</b>");

            OtherUtils.ShowOnGUI("Other small extensions",
                "generic <b>Swap(ref t1, ref t2)</b>\n" +
                "val.<b>ToShownMoney()</b> - formats money in common FriendsGames-accepted readable way\n" +
                "val.<b>ToStringWithSuffixes()</b> - format number like money, but without $\n" +
                "seconds.<b>ToShownTime()</b> - formats time in human-readable format\n" +
                "val.<b>ToShownPercents()</b> - formats number 0.541 to 54%");

            ToStringUtils.ShowOnGUI("<b>v.ToString(decimalPlaces)</b> - always uses . and not ,");

            EditorGUIUtils.LabelAtCenter("Common programming utils", FontStyle.Bold);

            CollectionUtils.ShowOnGUI("Lots of utils for collection. When Linq is not enough.",
                "items.<b>ClampedInd(i), ClampInd(i), IndIsValid(i)</b>, - clamp ind to valid range\n" +
                "items.<b>DebugIndIsValid(i)</b> will do logerror when index is invalid. You can put break there\n" +
                "items.<b>SortPartialOrder((item1,item2)=>item1>item2)</b> - sort when order is only partial\n" +
                "items.<b>SortBy(item=>item.ind), SortedBy()</b> - sorting by some number field\n" +
                "(15,25).<b>FromTo(i=>{})</b> - cool iterator\n" +
                "items.<b>PrintCollection(\",\")</b> - easily log collections\n" +
                "items.<b>Reversed(), FindIndex(), Find(), FindLastIndex(), Filter(), Contains(), Clone(), ConvertAll()</b>\n" +
                " - obvious, but work with any IEnumerable, just as it should\n" +
                "keyValuePair.Deconstruct<T, U>() - allows cool syntax like <b>foreach (var (key,value) in dict) { }</b>\n" +
                "items.<b>ForEach(), ForEachWithInd()</b> - iteration on ANY IEnumerable\n" +
                "also <b>ForEach<TEnum>(t=>{})</b> allow <b>all Enum values iteration</b>");

            Lens.ShowOnGUI("Lens implementation",
                "Its like dictionary, but two-way,\n" +
                "create it like <b>new Lens((i1,j1), (i2,j2), (i3,j1))</b>\n" +
                "then use like <b>t2 = lens[t1]</b>, <b>t1 = lens[t2]</b>. Also add and remove like dictionary");

            EditorGUIUtils.LabelAtCenter("MATH", FontStyle.Bold);

            GeometryUtils.ShowOnGUI("Geometry calculating library larva",
                "<b>CalcDistSqrToLine(pt, linePt1, linePt2)</b> - line considered infinite");

            MathUtils.ShowOnGUI("Lots of math utils, when Mathf and Vector3 are not enough",
                "if (val.<b>InRange</b>(5,10) {}\n" +
                "integer <b>Sign(val)</b>\n" +
                "Vector2 and Vector3 v.<b>ZTo0()</b>, v.<b>YToVal(y)</b> etc,\n" +
                "v.<b>IsSane()</b> checks vectors for NaN and infinities\n" +
                "v.<b>Clamp(min, max)</b> does component-vise clamps\n" +
                "val.<b>ToHash()</b>, val.ToHash(val2) combines simple types to long hashes\n" +
                "<b>MoveTowards(curr, tgt, maxDelta)</b> - double move towards\n" +
                "Also double <b>Clamp</b> and <b>Lerp</b>");

            MatrixUtils.ShowOnGUI("Matrix utils, when Unity's methods not enough",
                "m.<b>ExtractRotation()</b>, m.<b>ExtractPosition()</b>, m.<b>ExtractScale()</b>");

            RandomUtils.ShowOnGUI("Utils for random",
               "<b>RandomBetween(v1, v2)</b> - float random regardless of who is min or max\n" +
               "<b>Random(from, to)</b> - int/float random, including bounds\n" +
               "<b>RandomFromProbabilities(p0,p1,p2,...)</b> - returns 2 with probability weight p2\n" +
               "<b>Chance(0.8)</b> - returns true in 80% of cases\n" +
               "<b>list.RandomElement()</b>, <b>list.RandomInd()</b>");
        }
        protected override string docsURL => "https://docs.google.com/document/d/14rUt5Oh82YcYH_zBu6Ke0NeK4IYaLDu_OW7cS3UP3Lo/edit?usp=sharing";

        ExampleScript DefinesModifier = new ExampleScript("DefinesModifier");
        ExampleScript EditorUtils = new ExampleScript("EditorUtils");
        ExampleScript CompilationCallback = new ExampleScript("CompilationCallback");
        ExampleScript GUIUtils = new ExampleScript("GUIUtils");
        ExampleScript SettingsScriptableCreator = new ExampleScript("SettingsScriptableCreator");
        ExampleScript SettingsScriptable = new ExampleScript("SettingsScriptable");
        ExampleScript AnimUtils = new ExampleScript("AnimUtils");
        ExampleScript AsyncUtils = new ExampleScript("AsyncUtils");
        ExampleScript CollectionUtils = new ExampleScript("CollectionUtils");
        ExampleScript DestroyOnAwake = new ExampleScript("DestroyOnAwake");
        ExampleScript ExpFormula = new ExampleScript("ExpFormula");
        ExampleScript GeometryUtils = new ExampleScript("GeometryUtils");
        ExampleScript Lens = new ExampleScript("Lens");
        ExampleScript MathUtils = new ExampleScript("MathUtils");
        ExampleScript MatrixUtils = new ExampleScript("MatrixUtils");
        ExampleScript MonoBehaviourHasInstance = new ExampleScript("MonoBehaviourHasInstance");
        ExampleScript OtherUtils = new ExampleScript("OtherUtils");
        ExampleScript PlayerPrefsUtils = new ExampleScript("PlayerPrefsUtils");
        ExampleScript PrefabUtils = new ExampleScript("PrefabUtils");
        ExampleScript RandomUtils = new ExampleScript("RandomUtils");
        ExampleScript ReflectionUtils = new ExampleScript("ReflectionUtils");
        ExampleScript SerializationUtils = new ExampleScript("SerializationUtils");
        ExampleScript StringUtils = new ExampleScript("StringUtils");
        ExampleScript ToStringUtils = new ExampleScript("ToStringUtils");
        ExampleScript TransformUtils = new ExampleScript("TransformUtils");

    }
}