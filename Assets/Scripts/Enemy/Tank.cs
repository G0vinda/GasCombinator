namespace Enemy
{
    public class Tank : Enemy
    {
        private void Update()
        {
            if (!ProcessFreeze())
                return;
            
            FollowPlayer();
        }

        protected override void Unfreeze()
        {
            base.Unfreeze();
            StartWalkingAnimation();
        }
    }
}