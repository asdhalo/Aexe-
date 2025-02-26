using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Aexe.Resources
{
    public static class CreateDefaultImage
    {
        public static void CreateAndSave()
        {
            var resourcesDir = "Resources";
            if (!Directory.Exists(resourcesDir))
            {
                Directory.CreateDirectory(resourcesDir);
            }

            var imagePath = Path.Combine(resourcesDir, "no-image.png");
            using (var bitmap = new Bitmap(300, 300))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                // 设置高质量绘图
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                // 填充深灰色背景
                graphics.Clear(Color.FromArgb(30, 30, 30));

                // 绘制简单的图片占位符
                using (var pen = new Pen(Color.Gray, 2))
                {
                    // 绘制边框
                    graphics.DrawRectangle(pen, 50, 50, 200, 200);
                    // 绘制对角线
                    graphics.DrawLine(pen, 50, 50, 250, 250);
                    graphics.DrawLine(pen, 250, 50, 50, 250);
                }

                bitmap.Save(imagePath, ImageFormat.Png);
            }
        }
    }
} 