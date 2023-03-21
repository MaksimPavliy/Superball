#if ECS_ISO_HUMAN
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame.Iso
{
    public abstract class HumanView<THuman, TOrientedView> : MonoBehaviour
        where TOrientedView : OrientedHuman
        where THuman : struct, IComponentData
    {
        #region Orientation
        public Orientation orientation;
        public TOrientedView RT;
        public TOrientedView LB;
        public TOrientedView RB;
        public TOrientedView LT;
        public virtual TOrientedView GetOrientation(Orientation o)
        {
            switch (o)
            {
                default:
                case Orientation.RT: return RT;
                case Orientation.LB: return LB;
                case Orientation.LT: return LT;
                case Orientation.RB: return RB;
            }
        }
        #endregion

        #region Walking
        protected virtual float walkAnimSpeed => 2;
        float moveDist;
        Vector3 prevIsoCoo;
        [SerializeField] protected ImgAndShadow pic;
        void UpdateWalking(Entity e, THuman human)
        {
            moveDist += (prevIsoCoo - iso).ZTo0().magnitude;
            prevIsoCoo = iso;

            // Show animation with orientation.
            var moverExists = e.TryGetComponent<TrajectoryMover>(out var mover);
            if (moverExists && mover.isMoving)
            {
                // Rotate.
                orientation = OrientationUtils.GetMoveOrientation(mover, orientation);
                // Show walking anim.
                if (!mover.paused)
                {
                    var walkAnim = GetWalkingSprites(GetOrientation(orientation));
                    int ind = Mathf.RoundToInt(moveDist * walkAnimSpeed * walkAnim.Count) % walkAnim.Count;
                    ShowPic(currOriented => walkAnim[ind]);
                }
                else
                    ShowStanding(orientation);
            }
        }
        protected virtual List<SpriteAndShadow> GetWalkingSprites(TOrientedView orientation)
            => orientation.walk;
        #endregion

        #region Standing
        public void ShowStanding(Orientation orientation)
        {
            this.orientation = orientation;
            ShowStanding();
        }
        public void ShowStanding() => ShowPic(currOriented => currOriented.standing);
        #endregion

        Vector3 iso;
        public virtual void Show(Entity e, THuman human) {
            TrajectoryMoving.SetViewPosition(e, transform);
            iso = IsoCoos.WorldToIso(transform.position);
            UpdateWalking(e, human);
        }
        public void ShowAnimation(float startTime, Func<TOrientedView, List<SpriteAndShadow>> whatAnimation)
            => ShowAnimation(startTime, 1, whatAnimation);
        public void ShowAnimation(DurableProcess process,
            Func<TOrientedView, List<SpriteAndShadow>> whatAnimation, bool reverse = false)
            => Sprites.ShowAnimProcess(process, pic, whatAnimation(GetOrientation(orientation)), reverse);
        public void ShowAnimation(float startTime, float speed,
            Func<TOrientedView, List<SpriteAndShadow>> whatAnimation)
            => Sprites.ShowAnimProcess(pic, startTime, whatAnimation(GetOrientation(orientation)), speed);

        protected virtual void ShowAnimation(DurableProcess process,
            List<SpriteAndShadow> pics, bool reverse = false)
            => Sprites.ShowAnimProcess(process, pic, pics, reverse);
        protected virtual void ShowAnimation(float startTime, float speed,
            List<SpriteAndShadow> pics)
            => Sprites.ShowAnimProcess(pic, startTime, pics, speed);
        public virtual void ShowPic(Func<TOrientedView, SpriteAndShadow> whatPic)
            =>  pic.Show(whatPic(GetOrientation(orientation)));
    }
}
#endif