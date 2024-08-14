using KEngine.Components.DrawableComponents;
using KEngine.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KEngine.Components {
    public class AnimationController : Component {
        SpriteRenderer sr;
        Dictionary<string, Animation> animations = new();
        TimeSpan currentAnimationElapsedTime = TimeSpan.Zero;
        public Animation CurrentAnimation { get; private set; }
        int frameIndex = 0;
        int FrameIndex {
            get {
                return frameIndex;
            }
            set {

                frameIndex = value;

                if (frameIndex >= CurrentAnimation.Frames) {
                    SetAnimation(CurrentAnimation.NextAnimation, true);
                } else {
                    sr.spriteIndex = CurrentFrame.SpriteSheetIndex;
                }
            }
        }
        public Animation.Frame CurrentFrame => CurrentAnimation[FrameIndex];
        public string StartingAnimation { get; init; }
        public Animation[] Animations {
            set {
                foreach (var anim in value)
                {
                    animations[anim.Name] = anim;
                }
            }
        }
        public override void Initialize() {
            base.Initialize();
            sr = GameObject.GetComponent<SpriteRenderer>();

            SetAnimation(StartingAnimation);
            sr.spriteIndex = CurrentFrame.SpriteSheetIndex;
        }

        public void SetAnimation(string animationName, bool restartIfSameAnimation = false) {
            if (!restartIfSameAnimation && animationName == CurrentAnimation?.Name)
                return;
            CurrentAnimation = animations[animationName];
            currentAnimationElapsedTime = TimeSpan.Zero;
            FrameIndex = 0;
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
            currentAnimationElapsedTime += TimeSpan.FromSeconds(deltaTime);
            if (currentAnimationElapsedTime > CurrentFrame.Duration) {
                currentAnimationElapsedTime -= CurrentFrame.Duration;
                FrameIndex++;
            }
        }

    }
}
