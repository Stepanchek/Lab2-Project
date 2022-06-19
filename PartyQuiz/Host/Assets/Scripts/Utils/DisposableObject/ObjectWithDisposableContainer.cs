namespace PartyQuiz.Utils
{
    public class ObjectWithDisposableContainer : DisposableObject
    {
        protected readonly DisposableContainer DC = new();

        public void ShowGameObject()
        {
            gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public override void Dispose()
        {
            DC.Dispose();

            HideGameObject();
        }

        public void HideGameObject()
        {
            gameObject.SetActive(false);
        }
    }
}