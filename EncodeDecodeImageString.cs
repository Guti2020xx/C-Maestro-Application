using System.Drawing;

class ImageStringEncoderDecoder
{
    // adapted from stack overflow
    // https://stackoverflow.com/questions/21325661/convert-an-image-selected-by-path-to-base64-string
    public static string encodeImageToString(string filePath)
    {
        // arrays and string to save image into array and then saving it into a byte[] array 
        byte[] imageArr;
        string encodedString;
        try
        { // seeing if image file path can be found
            imageArr = File.ReadAllBytes(filePath);
            encodedString = Convert.ToBase64String(imageArr);
            return encodedString;
        }
        catch (Exception ex) // incase image is not found
        {
            MessageBox.Show("Error: " + ex.Message);
            return "";
        }
    }

    public static byte[] decodeBase64StringToImage( string base64String)
    {   // decoding from base 64 to conver to image
        byte[] img = Convert.FromBase64String(base64String);
        return img;
    }

    public static Image GetImage(byte[] Data, byte[] Default)
    {
        Image newImg;
        try
        {
            // generating image from the byte array data
            byte[] ImgData = (Data);

            using (MemoryStream ms = new MemoryStream(ImgData, 0, ImgData.Length))
            {
                ms.Write(ImgData, 0, ImgData.Length);
                newImg = Image.FromStream(ms, true);
            }

            return newImg;
        }
        catch(Exception e)
        {   
            using (MemoryStream ms = new MemoryStream(Default, 0, Default.Length))
            {
                ms.Write(Default, 0, Default.Length);
                newImg = Image.FromStream(ms, true);
            }

            return newImg;
        }
    }

    public static byte[] ImageBytes(Image image)
    {   //turns image to png format byte array developed by Ian
        MemoryStream ms = new MemoryStream();
        image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        return ms.ToArray();
    }

}