using Newtonsoft.Json.Linq;

namespace ImageUploaderLibrary
{
    public static class ImageUploader
    {
        const string API_KEY = "87c3ecadba19d1394aa1664628ca71b9";
        /// <summary>
        /// Загружает изображение в хранилище изображений imgBB
        /// </summary>
        /// <param name="imagePath">Путь к файлу с загружаемым изображением</param>
        /// <returns>
        ///     UploadResult.Success = true   -  если удалось загрузить изображение и получить ссылку на него, false в противном случае
        ///     UploadResult.Data     -    Ссылка на изображение, если Success = true и null если Success = false
        ///     UploadResult.Message      -    Информация об ошибке если Success = false и null если Success = true
        /// </returns>
        public static async Task<UploadResult<string>> UploadImageAsync(string imagePath)
        {
            var uploadResult = new UploadResult<string>();


            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string requestUrl = $"https://api.imgbb.com/1/upload?key={API_KEY}";
                    MultipartFormDataContent content = new MultipartFormDataContent();
                    string base64OfImage = Convert.ToBase64String(File.ReadAllBytes(imagePath));
                    content.Add(new StringContent(base64OfImage), "image");
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUrl) { Content = content };
                    HttpResponseMessage response = await client.SendAsync(request);
                    var responseJsonContent = await response.Content.ReadAsStringAsync();

                    JObject jsonObject = JObject.Parse(responseJsonContent);
                    var imageLink = jsonObject.SelectToken("data.url").ToString();
                    uploadResult.Success = true;
                    uploadResult.Data = imageLink;
                }
                catch
                {
                    uploadResult.Success = false;
                    uploadResult.Message = "Возникла ошибка при отправке изображения на сервер";
                }
            }

            return uploadResult;
        }
    }
}
