using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ConfigurationModels
{
    public class AwsS3Configuration
    {
        public string Section { get; set; } = "AWSS3Settings";
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
        public string RegionEndpoint { get; set; } = string.Empty;
    }
}
