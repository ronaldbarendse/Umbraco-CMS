using System;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors.ValueConverters;
using Umbraco.Web.Models;

namespace Umbraco.Web
{
    /// <summary>
    /// Provides extension methods for getting ImageProcessor URLs from the core Image Cropper property editor.
    /// </summary>
    public static class ImageCropperTemplateExtensions
    {
        /// <summary>
        /// Gets the ImageProcessor URL.
        /// </summary>
        /// <param name="mediaItem">The media item.</param>
        /// <param name="propertyAlias">The property alias.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="urlMode">The URL mode.</param>
        /// <param name="width">The width of the output image.</param>
        /// <param name="height">The height of the output image.</param>
        /// <param name="cropAlias">The crop alias.</param>
        /// <param name="useCropDimensions">Use crop dimensions to have the output image sized according to the predefined crop sizes, this will override the width and height parameters.</param>
        /// <param name="preferFocalPoint">Use focal point, to generate an output image using the focal point instead of the predefined crop.</param>
        /// <param name="imageCropMode">The image crop mode.</param>
        /// <param name="imageCropAnchor">The image crop anchor.</param>
        /// <param name="ratioMode">Use a dimension as a ratio.</param>
        /// <param name="furtherOptions">These are any query string parameters (formatted as query strings) that ImageProcessor supports. For example:
        /// <example><![CDATA[
        /// furtherOptions: "&bgcolor=fff"
        /// ]]></example></param>
        /// <param name="quality">Quality percentage of the output image.</param>
        /// <param name="cacheBuster">Add a serialized date of the last edit of the item to ensure client cache refresh when updated.</param>
        /// <param name="upScale">If the image should be upscaled to requested dimensions.</param>
        /// <returns>
        /// The ImageProcessor URL.
        /// </returns>
        /// <exception cref="ArgumentNullException">mediaItem</exception>
        public static string GetCropUrl(
            this IPublishedContent mediaItem,
            string culture = null,
            UrlMode urlMode = UrlMode.Default,
            string propertyAlias = Constants.Conventions.Media.File,
            int? width = null,
            int? height = null,
            string cropAlias = null,
            int? quality = null,
            bool? useCropDimensions = null,
            bool preferFocalPoint = false,
            ImageCropMode? imageCropMode = null,
            ImageCropAnchor? imageCropAnchor = null,
            ImageCropRatioMode? ratioMode = null,
            string furtherOptions = null,
            bool cacheBuster = true,
            bool upScale = true)
        {
            if (mediaItem == null) throw new ArgumentNullException(nameof(mediaItem));

            if (!mediaItem.HasProperty(propertyAlias) || !mediaItem.HasValue(propertyAlias))
            {
                return null;
            }

            var imageUrl = mediaItem.MediaUrl(culture, urlMode, propertyAlias);
            var cacheBusterValue = cacheBuster ? mediaItem.UpdateDate.ToFileTimeUtc().ToString(CultureInfo.InvariantCulture) : null;
            var imageCropperValue = mediaItem.Value<ImageCropperValue>(propertyAlias, culture);

            return GetCropUrl(imageUrl, imageCropperValue, width, height, cropAlias, quality, imageCropMode, imageCropAnchor, preferFocalPoint, useCropDimensions, cacheBusterValue, furtherOptions, ratioMode, upScale);
        }

        /// <summary>
        /// Gets the ImageProcessor URL.
        /// </summary>
        /// <param name="imageUrl">The image URL.</param>
        /// <param name="width">The width of the output image.</param>
        /// <param name="height">The height of the output image.</param>
        /// <param name="imageCropperValue">The JSON data from the Image Cropper property editor.</param>
        /// <param name="cropAlias">The crop alias.</param>
        /// <param name="quality">Quality percentage of the output image.</param>
        /// <param name="imageCropMode">The image crop mode.</param>
        /// <param name="imageCropAnchor">The image crop anchor.</param>
        /// <param name="preferFocalPoint">Use focal point to generate an output image using the focal point instead of the predefined crop if there is one.</param>
        /// <param name="useCropDimensions">Use crop dimensions to have the output image sized according to the predefined crop sizes, this will override the width and height parameters.</param>
        /// <param name="cacheBusterValue">Add a serialized date of the last edit of the item to ensure client cache refresh when updated.</param>
        /// <param name="furtherOptions">These are any query string parameters (formatted as query strings) that ImageProcessor supports. For example:
        /// <example><![CDATA[
        /// furtherOptions: "&bgcolor=fff"
        /// ]]></example></param>
        /// <param name="ratioMode">Use a dimension as a ratio.</param>
        /// <param name="upScale">If the image should be upscaled to requested dimensions.</param>
        /// <returns>
        /// The ImageProcessor URL.
        /// </returns>
        [Obsolete("We should limit the extension methods on strings!")]
        public static string GetCropUrl(
            this string imageUrl,
            int? width = null,
            int? height = null,
            string imageCropperValue = null,
            string cropAlias = null,
            int? quality = null,
            ImageCropMode? imageCropMode = null,
            ImageCropAnchor? imageCropAnchor = null,
            bool preferFocalPoint = false,
            bool? useCropDimensions = null,
            string cacheBusterValue = null,
            string furtherOptions = null,
            ImageCropRatioMode? ratioMode = null,
            bool upScale = true)
        {
            ImageCropperValue cropDataSet = null;
            if (!string.IsNullOrEmpty(imageCropperValue) && imageCropperValue.DetectIsJson() && (imageCropMode == ImageCropMode.Crop || imageCropMode == null))
            {
                try
                {
                    cropDataSet = JsonConvert.DeserializeObject<ImageCropperValue>(imageCropperValue, new JsonSerializerSettings
                    {
                        Culture = CultureInfo.InvariantCulture,
                        FloatParseHandling = FloatParseHandling.Decimal
                    });
                }
                catch (Exception ex)
                {
                    // FIXME Why is this logged? There's nothing we can do and this might get called quite often!
                    Current.Logger.Error(typeof(ImageCropperTemplateExtensions), ex, "Could not parse the JSON string: {Json}", imageCropperValue);
                }
            }

            return GetCropUrl(imageUrl, cropDataSet, width, height, cropAlias, quality, imageCropMode, imageCropAnchor, preferFocalPoint, useCropDimensions, cacheBusterValue, furtherOptions, ratioMode, upScale);
        }

        /// <summary>
        /// Gets the ImageProcessor URL.
        /// </summary>
        /// <param name="imageUrl">The image URL.</param>
        /// <param name="cropDataSet">The crop data set.</param>
        /// <param name="width">The width of the output image.</param>
        /// <param name="height">The height of the output image.</param>
        /// <param name="cropAlias">The crop alias.</param>
        /// <param name="quality">Quality percentage of the output image.</param>
        /// <param name="imageCropMode">The image crop mode.</param>
        /// <param name="imageCropAnchor">The image crop anchor.</param>
        /// <param name="preferFocalPoint">Use focal point to generate an output image using the focal point instead of the predefined crop if there is one.</param>
        /// <param name="useCropDimensions">Use crop dimensions to have the output image sized according to the predefined crop sizes, this will override the width and height parameters.</param>
        /// <param name="cacheBusterValue">Add a serialized date of the last edit of the item to ensure client cache refresh when updated.</param>
        /// <param name="furtherOptions">These are any query string parameters (formatted as query strings) that ImageProcessor supports. For example:
        /// <example><![CDATA[
        /// furtherOptions: "&bgcolor=fff"
        /// ]]></example></param>
        /// <param name="ratioMode">Use a dimension as a ratio.</param>
        /// <param name="upScale">If the image should be upscaled to requested dimensions.</param>
        /// <returns>
        /// The ImageProcessor URL.
        /// </returns>
        [Obsolete("We should limit the extension methods on strings!")]
        public static string GetCropUrl(
            this string imageUrl,
            ImageCropperValue cropDataSet,
            int? width = null,
            int? height = null,
            string cropAlias = null,
            int? quality = null,
            ImageCropMode? imageCropMode = null,
            ImageCropAnchor? imageCropAnchor = null,
            bool preferFocalPoint = false,
            bool? useCropDimensions = null,
            string cacheBusterValue = null,
            string furtherOptions = null,
            ImageCropRatioMode? ratioMode = null,
            bool upScale = true)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return null;
            }

            var imageProcessorUrl = new StringBuilder();

            if (cropDataSet != null && (imageCropMode == ImageCropMode.Crop || imageCropMode == null))
            {
                var crop = cropDataSet.GetCrop(cropAlias);
                var hasCropAlias = !string.IsNullOrWhiteSpace(cropAlias);

                if (crop == null && hasCropAlias)
                {
                    // If a crop was specified, but not found, return null
                    return null;
                }

                imageProcessorUrl.Append(imageUrl);
                cropDataSet.AppendCropBaseUrl(imageProcessorUrl, crop, !hasCropAlias, preferFocalPoint);

                if (crop != null && useCropDimensions.GetValueOrDefault(hasCropAlias))
                {
                    width = crop.Width;
                    height = crop.Height;
                }

                // If a predefined crop has been specified, there are no coordinates and no ratio mode, but a width parameter has been passed: we can get the crop ratio for the height
                if (crop != null && hasCropAlias && crop.Coordinates == null && ratioMode == null && width != null && height == null)
                {
                    var heightRatio = crop.Height / (decimal)crop.Width;
                    imageProcessorUrl.Append("&heightratio=" + heightRatio.ToString(CultureInfo.InvariantCulture));
                }

                // If a predefined crop has been specified, there are no coordinates and no ratio mode, but a height parameter has been passed: we can get the crop ratio for the width
                if (crop != null && hasCropAlias && crop.Coordinates == null && ratioMode == null && width == null && height != null)
                {
                    var widthRatio = crop.Width / (decimal)crop.Height;
                    imageProcessorUrl.Append("&widthratio=" + widthRatio.ToString(CultureInfo.InvariantCulture));
                }
            }
            else
            {
                imageProcessorUrl.Append(imageUrl);

                if (imageCropMode == null)
                {
                    imageCropMode = ImageCropMode.Pad;
                }

                imageProcessorUrl.Append("?mode=" + imageCropMode.ToString().ToLowerInvariant());

                if (imageCropAnchor != null)
                {
                    imageProcessorUrl.Append("&anchor=" + imageCropAnchor.ToString().ToLowerInvariant());
                }
            }

            // FIXME Presets can also contain formats, but are not recognized here! Why is this even done? Can't we always put the quality at the end?
            var hasFormat = furtherOptions != null && furtherOptions.InvariantContains("&format=");

            // Only put quality here, if we don't have a format specified, otherwise we need to put quality at the end to avoid it being overridden by the format
            if (quality != null && !hasFormat)
            {
                imageProcessorUrl.Append("&quality=" + quality);
            }

            if (width != null && ratioMode != ImageCropRatioMode.Width)
            {
                imageProcessorUrl.Append("&width=" + width);
            }

            if (height != null && ratioMode != ImageCropRatioMode.Height)
            {
                imageProcessorUrl.Append("&height=" + height);
            }

            if (ratioMode == ImageCropRatioMode.Width && height != null)
            {
                // If only height specified then assume a square
                if (width == null)
                {
                    width = height;
                }

                var widthRatio = (decimal)width / (decimal)height;
                imageProcessorUrl.Append("&widthratio=" + widthRatio.ToString(CultureInfo.InvariantCulture));
            }

            if (ratioMode == ImageCropRatioMode.Height && width != null)
            {
                // If only width specified then assume a square
                if (height == null)
                {
                    height = width;
                }

                var heightRatio = (decimal)height / (decimal)width;
                imageProcessorUrl.Append("&heightratio=" + heightRatio.ToString(CultureInfo.InvariantCulture));
            }

            if (upScale == false)
            {
                imageProcessorUrl.Append("&upscale=false");
            }

            if (furtherOptions != null)
            {
                imageProcessorUrl.Append(furtherOptions);
            }

            // If furtherOptions contains a format, we need to put the quality after the format.
            if (quality != null && hasFormat)
            {
                imageProcessorUrl.Append("&quality=" + quality);
            }

            if (cacheBusterValue != null)
            {
                imageProcessorUrl.Append("&rnd=" + cacheBusterValue);
            }

            return imageProcessorUrl.ToString();
        }
    }
}
