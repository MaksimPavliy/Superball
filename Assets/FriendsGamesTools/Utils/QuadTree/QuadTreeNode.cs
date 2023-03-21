using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools
{
    public class QuadTreeNode<TItem>
    {
        public List<TItem> items;
        public QuadTreeNode<TItem> XY { get => children[(int)QuadPos.XY]; set => children[(int)QuadPos.XY] = value; }
        public QuadTreeNode<TItem> Xy { get => children[(int)QuadPos.Xy]; set => children[(int)QuadPos.Xy] = value; }
        public QuadTreeNode<TItem> xY { get => children[(int)QuadPos.xY]; set => children[(int)QuadPos.xY] = value; }
        public QuadTreeNode<TItem> xy { get => children[(int)QuadPos.xy]; set => children[(int)QuadPos.xy] = value; }
        public readonly QuadTreeNode<TItem>[] children = new QuadTreeNode<TItem>[4];
        public Vector2 center;
        public float halfWidth;

        bool isMinimum => childHalfWidth < quadTree.minHalfWidth;
        public float childHalfWidth => halfWidth * 0.5f;
        float boundingRadius => halfWidth * 1.415f;

        public abstract class QuadTree
        {
            public float minHalfWidth { get; protected set; } // few times smaller than radius of common search.
            public abstract QuadTreeNode<TItem> CreateChild(QuadTreeNode<TItem> node, Vector2 pos);
            public abstract Vector2 GetPos(TItem item);
            public abstract bool Contains(QuadTreeNode<TItem> node, TItem item);
            public abstract void ClearChildren(QuadTreeNode<TItem> node);
        }
        QuadTree quadTree;
        public QuadTreeNode(QuadTree quadTree, int itemsStartCapacity = 0) {
            this.quadTree = quadTree;
            items = new List<TItem>(itemsStartCapacity);
        }
        public virtual void Init(Vector2 center, float halfWidth) {
            this.center = center;
            this.halfWidth = halfWidth;
        }
        public void Clear() {
            items.Clear();
            quadTree.ClearChildren(this);
        }
        public bool Contains(Vector2 pos)
            => Mathf.Abs(pos.x - center.x) < halfWidth && Mathf.Abs(pos.y - center.y) < halfWidth;

        public QuadTreeNode<TItem> GetChild(Vector2 pos) => children[(int)GetChildQuadPos(pos)];
        public QuadPos GetChildQuadPos(Vector2 pos) {
            var xPlus = pos.x > center.x;
            var yPlus = pos.y > center.y;
            if (xPlus) {
                if (yPlus)
                    return QuadPos.XY;
                else
                    return QuadPos.Xy;
            } else {
                if (yPlus)
                    return QuadPos.xY;
                else
                    return QuadPos.xy;
            }
        }
        public bool hasChildren => XY != null || Xy != null || xY != null || xy != null;
        public void Add(Vector2 itemPos, TItem item) {
            var addToCurr = isMinimum;
            if (!addToCurr) { 
                var child = GetChild(itemPos);
                if (child == null)
                    child = quadTree.CreateChild(this, itemPos);
                if (quadTree.Contains(child, item))
                    child.Add(itemPos, item);
                else
                    addToCurr = true;
            }
            if (addToCurr)
                items.Add(item);
        }
        public void Remove(Vector2 itemPos, TItem item) {
            if (items.Remove(item))
                return;
            GetChild(itemPos)?.Remove(itemPos, item);
        }
        public IEnumerable<TItem> GetAllItems() {
            for (int i = 0; i < items.Count; i++) {
                var item = items[i];
                if (item != null)
                    yield return item;
            }
            for (int i = 0; i < children.Length; i++) {
                var child = children[i];
                if (child == null) continue;
                foreach (var item in children[i].GetAllItems())
                    yield return item;
            }
        }
        public IEnumerable<TItem> GetCloseItems(Vector2 pos, float r) {
            // if outside return
            // if inside return all items recursively
            // if on edge return close items

            var distSqr = (center - pos).sqrMagnitude;
            var outsideRSqr = r + boundingRadius;
            outsideRSqr *= outsideRSqr;
            var outside = distSqr > outsideRSqr;
            if (outside)
                yield break;

            var insideRSqr = Mathf.Max(0, r - boundingRadius);
            insideRSqr *= insideRSqr;
            var inside = distSqr < insideRSqr;
            if (inside) {
                foreach (var item in GetAllItems())
                    yield return item;
            } else {
                // On edge.
                var rSqr = r * r;
                if (items.Count > 0) {
                    for (int i = 0; i < items.Count; i++) {
                        if ((quadTree.GetPos(items[i]) - pos).sqrMagnitude < rSqr)
                            yield return items[i];
                    }
                }
                foreach (var child in children) {
                    if (child == null) continue;
                    foreach (var item in child.GetCloseItems(pos, r))
                        yield return item;
                }
            }
        }
    }
}