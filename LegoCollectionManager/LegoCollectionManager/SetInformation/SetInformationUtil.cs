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

        public SetInformation GetSetInformation(int id) {
            dynamic DSI = GetDynamicSetInformation(id.ToString());

            int count = DSI?.hits?.total?.value ?? 0;

            if (count == 0) {
                return null;
            }

            dynamic info = DSI?.hits?.hits?[0];
            dynamic versionedInfo = info?._source?.product_versions?.Last; 
            dynamic buildingInstructions = versionedInfo?.building_instructions;
            dynamic themeInfo = info?._source?.themes?.Last;
            dynamic locale = info?._source?.locale?["en-us"];
            dynamic additionalData = locale?.additional_data;

            BuildingInstructions[] instructions = new BuildingInstructions[buildingInstructions?.Count ?? 0];

            for (int i = 0; i < instructions.Length; i++) {
                instructions[i] = new BuildingInstructions() {
                    Id = buildingInstructions[i]?.id,
                    SequenceNumber = buildingInstructions[i]?.sequence?.element,
                    SequenceTotal = buildingInstructions[i]?.sequence?.total,
                    FileUrl = buildingInstructions[i]?.file?.url,
                    ImageUrl = buildingInstructions[i]?.image?.url,                    
                };
            }

            LegoSetImage primaryImage = new LegoSetImage()
            {
                Uid = themeInfo?.primary_image?.uid,
                FileName = themeInfo?.primary_image?.filename,
                ImageType = themeInfo?.primary_image?.imageType,
                Url = themeInfo?.primary_image?.url,
            };

            LegoSetImage boxImage = new LegoSetImage() {
                Uid = additionalData?.box_image?.uid,
                FileName = additionalData?.box_image?.filename,
                ImageType = additionalData?.box_image?.imageType,
                Url = additionalData?.box_image?.url,
            };

            LegoSetImage[] images = new LegoSetImage[themeInfo?.images?.Count ?? 0];

            for (int i = 0; i < images.Length; i++) {
                images[i] = new LegoSetImage() { 
                    Uid = themeInfo?.images[i]?.uid,
                    FileName = themeInfo?.images[i]?.filename,
                    ImageType = themeInfo?.images[i]?.imageType,
                    Url = themeInfo?.images[i]?.url,
                };
            }

            LegoSetImage[] additionalImages = new LegoSetImage[additionalData?.additional_images?.Count ?? 0];

            for (int i = 0; i < additionalImages.Length; i++) {
                dynamic img = additionalData.additional_images[i];
                img = img?.grown_up_image ?? img?.kid_image;
                img = img?.image;
                
                additionalImages[i] = new LegoSetImage() {
                    FileName = img?.filename,
                    Uid = img?.uid,
                    Url = img?.url,
                };
            }

            var setInfo = new SetInformation()
            {
                Id = info._id,
                Name = locale?.display_title,
                Description = locale?.description,
                BulletPoints = ((string)locale?.bullet_points)?.Split("\r\n"),
                Headline = locale?.headline,
                OneLiner = locale?.one_liner,                
                PieceCount = versionedInfo?.piece_count,
                Instructions = instructions,
                PrimaryImage = primaryImage,
                BoxImage = boxImage,                
                Images = images,
                AdditionalImages = additionalImages,
            };

            return setInfo;
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
}
