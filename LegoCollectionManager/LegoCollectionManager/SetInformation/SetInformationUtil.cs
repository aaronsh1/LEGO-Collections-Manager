using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LegoCollectionManager.SetInformation
{
    public class SetInformationUtil
    {
        private static string LegoApiSearchUrl = "https://services.slingshot.lego.com/api/v4/lego_historic_product_read/_search";
        private string LegoApiKey = "p0OKLXd8US1YsquudM1Ov9Ja7H91jhamak9EMrRB";

        public SetInformationUtil() { }

        public SetInformationUtil(string apiKey) {
            this.LegoApiKey = apiKey;
        }

        //TODO: Support versioning with dash-ids: i.e. 60321-1
        public SetInformation GetSetInformation(string id) {
            dynamic DSI = GetDynamicSetInformation(id);

            int count = DSI?.hits?.total?.value ?? 0;

            if (count == 0) {
                return null;
            }

            //TODO: Check if all these fields exist
            dynamic info = DSI?.hits?.hits?[0];
            dynamic versionedInfo = info?._source?.product_versions?[0]; //TODO: Use version from dash-id
            dynamic buildingInstructions = versionedInfo?.building_instructions;
            dynamic themeInfo = info?._source?.themes?[0];
            dynamic additionalData = info?._source?.locale?["en-us"]?.additional_data;

            BuildingInstructions[] instructions = new BuildingInstructions[buildingInstructions.Count];

            for (int i = 0; i < instructions.Length; i++) {
                instructions[i] = new BuildingInstructions() {
                    Id = buildingInstructions[i].id,
                    SequenceNumber = buildingInstructions[i].sequence.element,
                    SequenceTotal = buildingInstructions[i].sequence.total,
                    FileUrl = buildingInstructions[i].file.url,
                    ImageUrl = buildingInstructions[i].image.url,                    
                };
            }

            LegoSetImage primaryImage = new LegoSetImage()
            {
                Uid = themeInfo.primary_image.uid,
                FileName = themeInfo.primary_image.filename,
                ImageType = themeInfo.primary_image.imageType,
                Url = themeInfo.primary_image.url,
            };

            LegoSetImage[] images = new LegoSetImage[themeInfo.images.Count];

            for (int i = 0; i < images.Length; i++) {
                images[i] = new LegoSetImage() { 
                    Uid = themeInfo.images[i].uid,
                    FileName = themeInfo.images[i].filename,
                    ImageType = themeInfo.images[i].imageType,
                    Url = themeInfo.images[i].url,
                };
            }

            LegoSetImage[] additionalImages = new LegoSetImage[additionalData?.additional_images?.Count ?? 0];

            for (int i = 0; i < additionalImages.Length; i++) {
                dynamic img = additionalData.additional_images[i];
                img = img.grown_up_image ?? img.kid_image;
                img = img.image;
                
                additionalImages[i] = new LegoSetImage() {
                    FileName = img.filename,
                    Uid = img.uid,
                    Url = img.url,
                };
            }

            return new SetInformation()
            {
                Id = info._id,
                Name = info._source.locale["en-us"].display_title,
                PieceCount = versionedInfo.piece_count,
                Instructions = instructions,
                PrimaryImage = primaryImage,
                Images = images,
                AdditionalImages = additionalImages,
            };
        }

        private static object GetSearchRequestBody(string id) {
            return new
            {
                from = 0,
                query = new
                {
                    @bool = new
                    {
                        filter = new object[0],
                        must = new object[] {
                            new {
                                term = new {
                                    product_number = id,
                                }
                            }
                        },
                        should = new object[0]
                    }
                },
                size = 1,
                _source = new string[] {
                  "product_number",
                  "locale.en-us",
                  "locale.en-us",
                  "market.us.skus.item_id",
                  "market.us.skus.item_id",
                  "availability",
                  "themes",
                  "product_versions",
                  "assets"
              },
            };
        }

        private dynamic GetDynamicSetInformation(string id)
        {
            var request = WebRequest.Create(LegoApiSearchUrl);
            request.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:91.0) Gecko/20100101 Firefox/91.0";
            request.Headers["x-api-key"] = LegoApiKey;
            request.Method = "POST";

            var PostData = GetSearchRequestBody(id);

            string json = JsonSerializer.Serialize(PostData);

            byte[] content = Encoding.ASCII.GetBytes(json);

            request.ContentType = "application/json";
            request.ContentLength = content.Length;

            Stream reqStream = request.GetRequestStream();
            reqStream.Write(content, 0, content.Length);

            try
            {
                var response = request.GetResponse();

                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                string jsonResponse = reader.ReadToEnd();
                File.WriteAllText("test.json", jsonResponse);

                dynamic resObject = JObject.Parse(jsonResponse);
                
                return resObject;

            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }

    public class SetInformation { 
        public string Id { get; init; }
        public string Name { get; init; }
        public int PieceCount { get; init; }
        public BuildingInstructions[] Instructions { get; init; }
        public LegoSetImage PrimaryImage { get; init; }
        public LegoSetImage[] Images { get; init; }
        public LegoSetImage[] AdditionalImages { get; init; }
        //TODO: Parse additional _source.locale and _source.assets fields (such as description)
    }

    public class BuildingInstructions { 
        public string Id { get; init; }
        public int SequenceNumber { get; init; }
        public int SequenceTotal { get; init; }
        public string ImageUrl { get; init; }
        public string FileUrl { get; init; }
    }

    public class LegoSetImage { 
        public string Uid { get; init; }
        public string FileName { get; init; }
        public string Url { get; init; }
        public string ImageType { get; init; } //TODO: Create an enum
    }
}
