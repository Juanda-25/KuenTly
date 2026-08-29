using KuenTly.Helpers;
using KuenTly.Services.Interfaces;

namespace KuenTly.Services.Implementations
{
    public class ArchivoGuardadorService : IArchivoGuardadorService
    {
        public async Task<bool> GuardarArchivoAsync(string rutaOrigen, string nombreSugerido)
        {
#if ANDROID
            var uriDestino = await SeleccionarDestinoAsync(nombreSugerido);
            if (uriDestino is null)
                return false;

            var contexto = Platform.AppContext;
            using var streamDestino = contexto.ContentResolver?.OpenOutputStream(uriDestino);
            if (streamDestino is null)
                return false;

            using var streamOrigen = File.OpenRead(rutaOrigen);
            await streamOrigen.CopyToAsync(streamDestino);

            return true;
#else
            await Task.CompletedTask;
            throw new NotSupportedException("Guardar con el selector nativo solo está disponible en Android por ahora.");
#endif
        }

#if ANDROID
        private static Task<global::Android.Net.Uri?> SeleccionarDestinoAsync(string nombreSugerido)
        {
            var intent = new global::Android.Content.Intent(global::Android.Content.Intent.ActionCreateDocument);
            intent.AddCategory(global::Android.Content.Intent.CategoryOpenable);
            intent.SetType("application/octet-stream");
            intent.PutExtra(global::Android.Content.Intent.ExtraTitle, nombreSugerido);

            var tarea = GuardadoArchivoBridge.EsperarSeleccionAsync();

            Platform.CurrentActivity?.StartActivityForResult(intent, GuardadoArchivoBridge.CodigoSolicitud);

            return tarea;
        }
#endif
    }
}