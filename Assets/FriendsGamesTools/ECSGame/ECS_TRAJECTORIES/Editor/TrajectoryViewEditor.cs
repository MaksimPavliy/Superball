#if ECS_TRAJECTORIES
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.Iso
{
    public abstract class TrajectoryViewEditor<T> : Editor
        where T:TrajectoryView
    {
        TrajectoryView tgt => (TrajectoryView)target;
        bool multipleObjects => base.targets.Length > 1;
        Targets _targets;
        Targets tgts => _targets ?? (_targets = new Targets(this));
        class Targets : IEnumerator<TrajectoryView>, IEnumerable<TrajectoryView>
        {
            Editor editor;
            public Targets(Editor editor) => this.editor = editor;
            int ind = -1;
            public TrajectoryView Current => (TrajectoryView)editor.targets[ind];
            object IEnumerator.Current => Current;
            public void Dispose() => Reset();
            public bool MoveNext() => editor.targets.Length > ++ind;
            public void Reset() => ind = -1;

            public IEnumerator<TrajectoryView> GetEnumerator() => this;
            IEnumerator IEnumerable.GetEnumerator() => this;
        }
        MonoScript tgtScript;
        private void OnEnable()
        {
            if (tgt.pts.Count == 0)
                InitDefaultPts();
            SceneView.duringSceneGui += DuringSceneGUI;
            tgtScript = MonoScript.FromMonoBehaviour((T)target);
        }
        private void OnDisable()
        {
            SceneView.duringSceneGui -= DuringSceneGUI;
        }
        void InitDefaultPts()
        {
            tgt.pts.Clear();
            tgt.pts.Add(new TrajectoryPt(defaultPt1));
            tgt.pts.Add(new TrajectoryPt(defaultPt2));
        }
        const float defaultLength = 500;
        private Vector3 defaultPt1 => tgt.transform.position - Vector3.right * defaultLength * 0.5f;
        private Vector3 defaultPt2 => tgt.transform.position + Vector3.right * defaultLength * 0.5f;
        static bool canCopyPasteTrajectoryData;
        void DuringSceneGUI(SceneView scene)
        {
            if (tgt != null && tgt.selectedPt != null && tgt.selectedPt.posTransform == null)
            {
                if (!canCopyPasteTrajectoryData)
                    SetTransformPos(tgt.transform.position);
                else
                    tgt.transform.position = tgt.selectedPt.pos;
            }
        }
        void OnSceneGUI() { }
        void SetSelectedPtInd(int ind)
        {
            tgt.selectedPtInd = ind;
            if (tgt.selectedPt != null)
                SetTransformPos(tgt.selectedPt.pos);
        }
        private void SetTransformPos(Vector3 pos)
        {
            var worldPts = tgt.pts.ConvertAll(pt => pt.pos);
            tgt.transform.position = pos;
            if (tgt.selectedPt != null)
                tgt.selectedPt.SetPos(pos);

            // If pts are child transforms, we need to restore their positions.
            for (int i=0;i<tgt.pts.Count;i++)
            {
                if (tgt.pts[i].posTransform != null && tgt.selectedPtInd != i)
                    tgt.pts[i].posTransform.position = worldPts[i];
            }
        }
        void AddPtOnLineAfter(int ptInd)
        {
            // Create new pt.
            Vector3 prevPt;
            if (ptInd >= 0)
                prevPt = tgt.pts[ptInd].pos;
            else if (tgt.pts.Count >= 2)
                prevPt = (tgt.pts[0].pos + tgt.pts[1].pos) * 0.5f;
            else
                prevPt = defaultPt1;
            Vector3 nextPt;
            if (ptInd < 0)
                nextPt = defaultPt2;
            else if (ptInd < tgt.pts.Count - 1)
                nextPt = tgt.pts[ptInd + 1].pos;
            else if (tgt.pts.Count >= 2)
                nextPt = (tgt.pts[tgt.pts.Count - 1].pos + tgt.pts[tgt.pts.Count - 2].pos) * 0.5f;
            else
                nextPt = defaultPt2;

            var lineCenter = (prevPt + nextPt) * 0.5f;
            var pt = new TrajectoryPt(lineCenter);
            if (tgt.pts.IndIsValid(ptInd + 1))
                tgt.pts.Insert(ptInd + 1, pt);
            else
                tgt.pts.Add(pt);
            SetSelectedPtInd(ptInd + 1);
            Redraw();
        }
        void DeletePt(int ind)
        {
            tgt.pts[ind].Destroy();
            tgt.pts.RemoveAt(ind);
            if (tgt.selectedPtInd >= ind)
                tgt.selectedPtInd--;
            SetSelectedPtInd(tgt.selectedPtInd);
            Redraw();
        }
        void ShiftSelectedPt(int shift)
        {
            SetSelectedPtInd((tgt.selectedPtInd + shift + tgt.pts.Count) % tgt.pts.Count);
            Redraw();
        }
        void Redraw() => EditorUtility.SetDirty(tgt);
        void AddTransform(int ptInd)
        {
            tgt.pts[ptInd].AddTransform(tgt.transform.parent);
            Redraw();
        }
        void AddTransform(int ptInd, Transform tr) => AddTransform(tgt, ptInd, tr);
        void AddTransform(TrajectoryView tgt, int ptInd, Transform tr)
        {
            tgt.pts[ptInd].SetPos(tr);
            Redraw();
        }
        public override void OnInspectorGUI()
        {
            if (multipleObjects)
                OnMultipleTargetsInspectorGUI();
            else
                OnSingleTargetInspectorGUI();
        }
        void ShowInd(string ind)
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField(tgtScript, typeof(T), false);
            GUILayout.Label($"ind = {ind}");
            GUI.enabled = true;
        }
        public void OnMultipleTargetsInspectorGUI()
        {
            ShowInd(tgts.ConvertAll(t => t.ind).PrintCollection(","));
            bool changed = false;
            if (EditorGUIUtils.ColorField("color", tgts, t => t.col, (t, value) => t.col = value, ref changed))
                Redraw();

            // Edit multiple points.
            var sameCount = tgts.All(t => t.pts.Count == tgt.pts.Count);
            if (!sameCount)
                EditorGUILayout.LabelField("different pts count");
            else
            {
                for (int i = 0; i < tgt.pts.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUIUtils.ObjectField("", tgts, tgt => tgt.pts[i].posTransform, (tgt, tr) => AddTransform(tgt, i, tr), ref changed);
                    GUILayout.EndHorizontal();
                }
            }
        }
        static bool showDetails;
        public void OnSingleTargetInspectorGUI()
        {
            ShowInd(tgt.ind.ToString());
            if (!tgt.pts.IndIsValid(tgt.selectedPtInd))
                SetSelectedPtInd(0);
            GUILayout.BeginHorizontal();
            var newCol = EditorGUILayout.ColorField("color", tgt.col);
            if (newCol != tgt.col)
            {
                tgt.col = newCol;
                Redraw();
            }
            var newDraw = GUILayout.Toggle(tgt.draw, "draw");
            if (newDraw != tgt.draw)
            {
                tgt.draw = newDraw;
                Redraw();
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Label($"{tgt.pts.Count} pts");
            GUILayout.Label($"total length = {tgt.CalcTotalLength()}");

            if (GUILayout.Button("Add pt to start"))
                AddPtOnLineAfter(-1);

            for (int i = 0; i < tgt.pts.Count; i++)
            {
                // Draw header.
                GUILayout.BeginHorizontal();
                var wasSelected = i == tgt.selectedPtInd;
                bool isSelected = GUILayout.Toggle(wasSelected, $"{i}");
                if (isSelected && !wasSelected)
                    SetSelectedPtInd(i);
                Transform tr = EditorGUILayout.ObjectField(tgt.pts[i].posTransform, typeof(Transform), true) as Transform;
                if (tr != tgt.pts[i].posTransform)
                    AddTransform(i, tr);
                if (isSelected)
                {
                    if (GUILayout.Button("Add tr"))
                        AddTransform(tgt.selectedPtInd);
                    if (GUILayout.Button("Ins pt"))
                        AddPtOnLineAfter(tgt.selectedPtInd);
                }
                GUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add pt to end"))
                AddPtOnLineAfter(tgt.pts.Count - 1);

            GUILayout.Space(10);
            void EditPosShift(string name, Func<TrajectoryPt> getNearPt)
            {
                var nearPt = getNearPt();
                if (nearPt != null && tgt.selectedPt != null)
                {
                    var posCurr = tgt.GetGameSpacePos(tgt.selectedPt);
                    var posNear = tgt.GetGameSpacePos(nearPt);
                    var posShift = posCurr - posNear;
                    posShift = posShift.RemoveSmallComponents(0.001f);
                    Vector3 newPosShift = EditorGUILayout.Vector3Field(name, posShift);
                    if (newPosShift != posShift)
                    {
                        posCurr = newPosShift + posNear;
                        tgt.SetGameSpacePos(tgt.selectedPt, posCurr);
                        if (tgt.selectedPt.posTransform == null)
                            SetTransformPos(tgt.selectedPt.pos);
                        Redraw();
                    }
                }
            }

            EditorGUIUtils.ShowOpenClose(ref showDetails, "details");
            if (showDetails)
            {
                //GUILayout.Label($"selected pt ind = {tgt.selectedPtInd}");
                var posPrev = tgt.GetGameSpacePos(tgt.selectedPt);
                Vector3 pos = EditorGUILayout.Vector3Field($"{tgt.gameSpaceName} pos", posPrev);
                if (pos != posPrev)
                {
                    tgt.SetGameSpacePos(tgt.selectedPt, pos);
                    Redraw();
                }
                EditPosShift($"{tgt.gameSpaceName} shift prev", () => tgt.prevPt);
                EditPosShift($"{tgt.gameSpaceName} shift next", () => tgt.nextPt);

                GUILayout.Space(10);

                var changed = false;
                if (EditorGUIUtils.Toggle("dont kill close", ref tgt.dontKillClose, ref changed))
                    Redraw();
                EditorGUIUtils.Toggle("copy-paste", ref canCopyPasteTrajectoryData, ref changed);
                changed = false;
                EditorGUIUtils.Toggle("no next trajectory", ref tgt.noNextTrajectory, ref changed);
                if (!tgt.noNextTrajectory)
                    EditorGUIUtils.List("not next", tgt.notNext, true, ref changed);
                if (changed)
                    EditorUtils.SetDirty(tgt);
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Delete selected pt"))
                DeletePt(tgt.selectedPtInd);
        }
    }
}
#endif