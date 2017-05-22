using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Path = System.IO.Path;

namespace PdfImgExtractor
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var pdfPath = "E:\\Embryo_Omics\\pdfExtract\\Binder1.pdf";
            var outputPath = "E:\\Embryo_Omics\\SingleSection_OldGrant\\OutTest\\Pngs";

            PdfReader pdf = new PdfReader(pdfPath);
            PdfReaderContentParser parser = new PdfReaderContentParser(pdf);
            MyImageRenderListener listener = new MyImageRenderListener();
            for (int i = 1; i <= pdf.NumberOfPages; i++)
            {
                parser.ProcessContent(i, listener); //Runs MyImageRenderListener below
            }
            var imgCount = listener.Images.Count;
            for (int i = 0; i < imgCount; i++)
            {
                var image = listener.Images[i];
                var ext = listener.ImageExt[i];
                var name = listener.ImageNames[i];

                using (MemoryStream ms = new MemoryStream(image))
                {
                    using (System.Drawing.Image img = System.Drawing.Image.FromStream(ms))
                    {
                        img.Save(String.Format("{0}\\{1}.png", outputPath, name), ImageFormat.Png);
                    }
                }

            }

        }

        public static System.Drawing.Imaging.ImageCodecInfo GetImageEncoder(string imageType)
        {
            imageType = imageType.ToUpperInvariant();

            foreach (ImageCodecInfo info in ImageCodecInfo.GetImageEncoders())
            {
                if (info.FormatDescription == imageType)
                {
                    return info;
                }
            }
            return null;
        }

        public class MyImageRenderListener : IRenderListener
        {
            public List<byte[]> Images = new List<byte[]>();
            public List<string> ImageNames = new List<string>();
            public List<string> ImageExt = new List<string>();

            public void RenderText(TextRenderInfo renderInfo)
            {
                var text = renderInfo.GetText();
                if (text.Equals(" ") || text.Contains(" mz"))
                {
                    
                }
                else
                {
                    string mz = text.Split(',')[0];
                    string sample = text.Split(',')[1];
                    var name = String.Format("{0}_{1}", sample, mz);
                    ImageNames.Add(name);
                }
            }

            public void BeginTextBlock()
            {
            }

            public void EndTextBlock()
            {
            }

            public void RenderImage(ImageRenderInfo renderInfo)
            {
                PdfImageObject image = renderInfo.GetImage();
                try
                {
                    image = renderInfo.GetImage();
                    if (image == null) return;
                    using (MemoryStream ms = new MemoryStream(image.GetImageAsBytes()))
                    {
                        Images.Add(ms.ToArray());
                        ImageExt.Add(image.GetFileType());
                    }
                }
                catch (IOException ie)
                {
                /*
                * pass-through; image type not supported by iText[Sharp]; e.g. jbig2
                */
                }
            }

        }
    }
}
