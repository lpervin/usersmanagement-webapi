using System;
using System.IO;

using System.Threading.Tasks;
using System.Net.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Xunit;

namespace users_webapi_tests;

public class FakerTest
{
    /*[Fact]
    public void GenerateImages()
    {

        int width = 100;
        int height = 100;

        using var image = new Image<Rgba32>(width, height);
        var random = new Random();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var color = new Rgba32((byte) random.Next(256), (byte) random.Next(256), (byte) random.Next(256));
                image[x, y] = color;
            }
        }

        // Add random noise to the image
        image.Mutate(img => img.GaussianSharpen(random.Next(5, 20)));

        // Save the image to a file
        using var outputStream = new MemoryStream();
        image.Save(outputStream, new JpegEncoder());
        File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "random_image.jpg"), outputStream.ToArray());
        Assert.True(true);
    }

    enum ShapeType
    {
        Circle,
        Ellipse,
        Rectangle,
        RoundedRectangle,
        Triangle,
        Star
    }*/

    [Fact]
    public async Task GenerateColorfulImages()
    {
        int width = 100;
        int height = 100;

        // Load the list of product names and categories
        string[] productNames = {"Widget", "Gizmo", "Thingamabob", "Doohickey", "Whatsit"};
        string[] categories = {"Electronics", "Clothing", "Home Goods", "Toys", "Sporting Goods"};

        // Create an HttpClient to make requests to the random image generator API
        using var httpClient = new HttpClient();

        // Loop through each product and generate a random image
        foreach (var productName in productNames)
        {
            // Generate a random color for the background
            //var bgColor = new Rgba32(GetRandomByte(), GetRandomByte(), GetRandomByte());
            // Define the blue color
            var blueColor = new Rgba32(173, 216, 230);
            using var image = new Image<Rgba32>(width, height);

            // Fill the background with the random color
            image.Mutate(x => x.BackgroundColor(blueColor));

            // Get a random image from the API
            var randomImage = GetRandomImage(httpClient).Result;

            // Calculate the position and size of the random image in the product icon area
            int iconWidth = width / 2;
            int iconHeight = height / 2;
            int iconX = (width - iconWidth) / 2;
            int iconY = (height - iconHeight) / 2;

            // Load the random image into a stream and decode it into an ImageSharp image
            using var stream = httpClient.GetStreamAsync(randomImage).Result;
            using var randomImageSharp = Image.Load(stream);

            // Resize the random image to fit in the product icon area
            randomImageSharp.Mutate(x => x.Resize(new Size(iconWidth, iconHeight)));

            // Draw the random image onto the product icon image
            image.Mutate(x => x.DrawImage(randomImageSharp, new Point(iconX, iconY), 1f));

            // Save the image with the product name and category as the file name
            var fileName = $"{productName} - {categories[GetRandomNumber(0, categories.Length)]}.jpg";
            using var fs = new FileStream(fileName, FileMode.Create);
            image.SaveAsJpeg(fs);

        }

        static int GetRandomNumber(int min, int max)
        {
            var random = new Random();
            return random.Next(min, max);
        }

        static byte GetRandomByte()
        {
            var random = new Random();
            return (byte) random.Next(256);
        }

        static async Task<string> GetRandomImage(HttpClient httpClient)
        {
            // Make a request to the random image generator API
            var response = await httpClient.GetAsync("https://source.unsplash.com/random");

            // Get the URL of the random image from the API response
            var imageUrl = response.RequestMessage.RequestUri.ToString();

            return imageUrl;
        }
    }

}
    


