using PdfSharp.Fonts;

namespace KuenTly.Helpers
{
    // Le enseña a PDFsharp de dónde sacar las tipografías en Android,
    // reutilizando los mismos archivos .ttf que ya usa el resto de la app.
    public class KuenTlyFontResolver : IFontResolver
    {
        private byte[]? _regularFontData;
        private byte[]? _boldFontData;

        public bool Inicializado => _regularFontData is not null && _boldFontData is not null;

        public async Task InicializarAsync()
        {
            _regularFontData = await LeerFuenteAsync("OpenSans-Regular.ttf");
            _boldFontData = await LeerFuenteAsync("OpenSans-Semibold.ttf");
        }

        private static async Task<byte[]> LeerFuenteAsync(string nombreArchivo)
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(nombreArchivo);
            using var memoria = new MemoryStream();
            await stream.CopyToAsync(memoria);
            return memoria.ToArray();
        }

        public byte[] GetFont(string faceName)
        {
            return faceName switch
            {
                "OpenSansRegular" => _regularFontData ?? throw new InvalidOperationException("La fuente aún no se ha inicializado."),
                "OpenSansBold" => _boldFontData ?? throw new InvalidOperationException("La fuente aún no se ha inicializado."),
                _ => throw new InvalidOperationException($"Fuente no reconocida: {faceName}")
            };
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            var faceName = isBold ? "OpenSansBold" : "OpenSansRegular";
            return new FontResolverInfo(faceName);
        }
    }
}