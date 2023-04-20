using System.Drawing;
using System.Drawing.Drawing2D;

namespace iRacingDirector.Plugin.StandardOverlays
{
    public partial class MyPlugin
    {
        public Driver CamDriver;

        void DrawCurrentDriverRow()
        {
            var position = CamDriver.Position != null ? CamDriver.Position.Value.ToString() : "";
            var indicator = CamDriver.Position != null ? CamDriver.Position.Value.Ordinal() : "";

            var offset = 5;

            Graphics.InRectangle(80, 840, 140, 40)
                .WithBrush(Styles.YellowBrush)
                .WithPen(Styles.BlackPen)
                .DrawRectangleWithBorder()
                .WithFontSizeOf(24)
                .WithBrush(Styles.BlackBrush)
                .WithStringFormat(StringAlignment.Near)
                .Center(cg => cg
                            .DrawText(position, topOffset: offset)
                            .AfterText(position)
                            .MoveRight(3)
                            .WithFont(Settings.FontName, 18, FontStyle.Bold)
                            .DrawText(indicator, topOffset: offset)
                )

                .ToRight(width: 70)
                .WithLinearGradientBrush(Styles.White, Styles.WhiteSmoke, LinearGradientMode.BackwardDiagonal)
                .DrawRectangleWithBorder()
                .WithStringFormat(StringAlignment.Center)
                .WithBrush(Styles.BlackBrush)
                .DrawText(CamDriver.CarNumber, topOffset: offset)

                .ToRight(width: 400)
                .WithLinearGradientBrush(Styles.White, Styles.WhiteSmoke, LinearGradientMode.BackwardDiagonal)
                .DrawRectangleWithBorder()
                .WithStringFormat(StringAlignment.Center)
                .WithBrush(Styles.BlackBrush)
                .DrawText(CamDriver.UserName, topOffset: offset);
        }
    }
}
