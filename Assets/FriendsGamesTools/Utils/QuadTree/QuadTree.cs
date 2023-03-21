using System;
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools
{
    public abstract class QuadTree<TItem> : QuadTree<TItem, QuadTreeNode<TItem>>
    {
        protected QuadTree(float minHalfWidth) : base(minHalfWidth) { }
        protected override QuadTreeNode<TItem> CreateNode()
            => new QuadTreeNode<TItem>(this);
    }
    public abstract class QuadTree<TItem, TNode> : QuadTreeNode<TItem>.QuadTree
        where TNode : QuadTreeNode<TItem>
    {
        protected int startNodeCapacity;
        public QuadTree(float minHalfWidth, int nodesPoolItemsCount = 100, int startNodeCapacity = 2) {
            this.minHalfWidth = minHalfWidth;
            this.startNodeCapacity = startNodeCapacity;
            nodesPool = new Pool<TNode>(nodesPoolItemsCount, CreateNode);
        }

        public Pool<TNode> nodesPool;
        protected TNode root;
        protected abstract TNode CreateNode();
        private TNode CreateNode(Vector2 center, float halfWidth) {
            var node = nodesPool.Get();
            node.Init(center, halfWidth);
            return node;
        }
        public void Add(TItem item) {
            var itemPos = GetPos(item);
            if (root == null)
                root = CreateNode(Vector2.zero, minHalfWidth * 4 + 1.97f * Mathf.Max(Mathf.Abs(itemPos.x), Mathf.Abs(itemPos.y)));
            while (!Contains(root, item))
                root = CreateParent(root, itemPos);
            root.Add(itemPos, item);
        }
        public void Remove(TItem item) {
            if (root == null) return;
            root.Remove(GetPos(item), item);
        }

        public IEnumerable<TItem> GetCloseItems(Vector2 pos, float r) {
            if (root == null) yield break;
            foreach (var item in root.GetCloseItems(pos, r))
                yield return item;
        }

        public TNode CreateParent(TNode currQuad, Vector2 itemPos) {
            var parentHalfWidth = currQuad.halfWidth * 2;
            var parentPosShift = new Vector2(itemPos.x > currQuad.center.x ? 1 : -1, itemPos.y > currQuad.center.y ? 1 : -1) * currQuad.halfWidth;
            var parent = CreateNode(currQuad.center + parentPosShift, parentHalfWidth);
            parent.children[(int)parent.GetChildQuadPos(currQuad.center)] = currQuad;
            return parent;
        }
        public override QuadTreeNode<TItem> CreateChild(QuadTreeNode<TItem> node, Vector2 pos) {// => CreateChildren(node, null, null, null, null);
            var center = node.center;
            var childHalfWidth = node.childHalfWidth;
            var quadPos = node.GetChildQuadPos(pos);
            QuadTreeNode<TItem> newNode = null;
            switch (quadPos) {
                case QuadPos.XY: node.XY = newNode = CreateNode(center + new Vector2(1, 1) * childHalfWidth, childHalfWidth); break;
                case QuadPos.Xy: node.Xy = newNode = CreateNode(center + new Vector2(1, -1) * childHalfWidth, childHalfWidth); break;
                case QuadPos.xY: node.xY = newNode = CreateNode(center + new Vector2(-1, 1) * childHalfWidth, childHalfWidth); break;
                case QuadPos.xy: node.xy = newNode = CreateNode(center + new Vector2(-1, -1) * childHalfWidth, childHalfWidth); break;
            }
            return newNode;
        }

        void DestroyNode(TNode node) {
            node.Clear();
            nodesPool.Return(node);
        }
        public void Clear() {
            if (root == null) return;
            DestroyNode(root);
            root = null;
        }
        public override void ClearChildren(QuadTreeNode<TItem> node) {
            for (int i = 0; i < 4; i++) {
                var child = node.children[i];
                if (child == null) continue;
                DestroyNode((TNode)child);
                node.children[i] = null;
            }
        }

        public void IteratePartialOpening(Action<TNode> onClosedNodeFound, Action<TItem> onItemFound, Func<TNode, bool> openingCondition) {
            if (root == null) return;
            IteratePartialOpening(root, onClosedNodeFound, onItemFound, openingCondition);
        }
        private void IteratePartialOpening(TNode node, Action<TNode> onClosedNodeFound, Action<TItem> onItemFound, Func<TNode, bool> openingCondition) {
            if (!openingCondition(node))
                onClosedNodeFound(node);
            else {
                foreach (var item in node.items)
                    onItemFound(item);
                for (int i = 0; i < node.children.Length; i++) {
                    var child = node.children[i] as TNode;
                    if (child != null)
                        IteratePartialOpening(child, onClosedNodeFound, onItemFound, openingCondition);
                }
            }
        }

        public void DebugDraw(bool drawQuads, bool drawItems, Func<Vector2, Vector3> transform,
            float vertexSize, Color quadColor, Color itemColor) {

            void Draw(TNode node) {
                if (node == null) return;
                if (drawQuads) {
                    var X = new Vector2(node.halfWidth, 0);
                    var Y = new Vector2(0, node.halfWidth);
                    var XY = transform(node.center + X + Y);
                    var Xy = transform(node.center + X - Y);
                    var xY = transform(node.center - X + Y);
                    var xy = transform(node.center - X - Y);
                    Debug.DrawLine(XY, xY, quadColor);
                    Debug.DrawLine(XY, Xy, quadColor);
                    Debug.DrawLine(xy, xY, quadColor);
                    Debug.DrawLine(xy, Xy, quadColor);
                }
                if (drawItems)
                    node.items.ForEach(item => DebugDrawUtils.DrawCross(transform(GetPos(item)), vertexSize, itemColor));
                for (int i = 0; i < node.children.Length; i++)
                    Draw(node.children[i] as TNode);
            }
            Draw(root);
        }
    }
    public enum QuadPos { XY = 0, Xy = 1, xY = 2, xy = 3 }
}