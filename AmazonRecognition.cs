using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Image = Amazon.Rekognition.Model.Image;

namespace AmazonImgRecognition_WF
{
    internal class AmazonRecognition
    {
        private string accessKey = "AKIA6ODUZCHYJDSZJ3WG";
        private string secretKey = "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";
        private static string bucketName = "myrekognition1";
        private static AmazonRecognition _instance;
        private static AmazonRekognitionClient rekognitionClient;
        private static AmazonS3Client s3Client;

        private AmazonRecognition()
        {
            BasicAWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);
            s3Client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.EUWest1);
            rekognitionClient = new AmazonRekognitionClient(credentials, Amazon.RegionEndpoint.EUWest1);
        }

        public static AmazonRecognition GetClient()
        {
            if (_instance == null)
            {
                _instance = new AmazonRecognition();
            }
            return _instance;
        }

        public static void UploadFileToBucket(string filePath, string fileName)
        {
            try
            {
                
                // Upload the file to Amazon S3
                TransferUtility fileTransferUtility = new TransferUtility(s3Client);
                fileTransferUtility.Upload(filePath, bucketName, fileName);
                Console.WriteLine("Upload completed!");

            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
        }

        public static async Task<System.Drawing.Image> GetRecognition(string photoName, System.Drawing.Image image)
        {
            
            Bitmap bmp = new Bitmap(image);
            DetectTextRequest detectTextRequest = new DetectTextRequest()
            {
                Image = new Image()
                {
                    S3Object = new S3Object()
                    {
                        Name = photoName,
                        Bucket = bucketName
                    }
                }
            };

            try
            {
                
                DetectTextResponse detectTextResponse = await rekognitionClient.DetectTextAsync(detectTextRequest);
                List<TextDetection> textDetections = detectTextResponse.TextDetections;


                using (Graphics g = Graphics.FromImage(bmp))
                using (Pen pen = new Pen(Color.Red, 2))  // Use a red pen with thickness 2
                {
                    foreach (TextDetection text in textDetections)
                    {
                        BoundingBox box = text.Geometry.BoundingBox;

                        int left = (int)(box.Left * bmp.Width);
                        int top = (int)(box.Top * bmp.Height);
                        int width = (int)(box.Width * bmp.Width);
                        int height = (int)(box.Height * bmp.Height);

                        Rectangle rect = new Rectangle(left, top, width, height);

                        g.DrawRectangle(pen, rect);
                    }
                }

                return bmp;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return bmp;
            }
        }
    }
}
