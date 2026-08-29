namespace KuenTly.Helpers
{
    // Puente entre el resultado del selector nativo de Android (que llega por callback)
    // y el código async/await del resto de la app.
    public static class GuardadoArchivoBridge
    {
        public const int CodigoSolicitud = 4200;

        private static TaskCompletionSource<global::Android.Net.Uri?>? _tcs;

        public static Task<global::Android.Net.Uri?> EsperarSeleccionAsync()
        {
            _tcs = new TaskCompletionSource<global::Android.Net.Uri?>();
            return _tcs.Task;
        }

        public static void ResolverResultado(int requestCode, global::Android.App.Result resultCode, global::Android.Content.Intent? data)
        {
            if (requestCode != CodigoSolicitud || _tcs is null)
                return;

            if (resultCode == global::Android.App.Result.Ok && data?.Data is not null)
                _tcs.TrySetResult(data.Data);
            else
                _tcs.TrySetResult(null);

            _tcs = null;
        }
    }
}