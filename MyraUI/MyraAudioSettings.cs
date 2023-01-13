using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL.MyraUI
{
    public static class MyraAudioSettings
    {
        public static Grid grid;
        static HorizontalSlider SFXSlider;
        static HorizontalSlider MusicSlider;
        public static void Setup()
        {

            grid = MyraMain.BaseGrid();

            grid.ColumnsProportions.Add(new Proportion());

            // Add widgets
            var SFX = new Grid();
            SFX.ColumnsProportions.Add(new Proportion());
            SFX.RowsProportions.Add(new Proportion());

            var SFXlabel = new Label();
            SFXlabel.Text = "SFX";
            SFXlabel.GridColumn = 0;
            SFX.Widgets.Add(SFXlabel);

            SFXSlider = new HorizontalSlider();
            SFXSlider.GridColumn = 1;
            SFXSlider.Value = AssetManager.defaultVolume * 100f;
            SFXSlider.ValueChanged += (s, a) =>
            {
                AssetManager.defaultVolume = SFXSlider.Value/100f;
            };
            SFX.Widgets.Add(SFXSlider);

            MyraMain.StandardAlignment(grid, SFX);

            var Music = new Grid();
            Music.ColumnsProportions.Add(new Proportion());
            Music.RowsProportions.Add(new Proportion());

            var MusicLabel = new Label();
            MusicLabel.Text = "Music";
            MusicLabel.GridColumn = 0;
            Music.Widgets.Add(MusicLabel);

            MusicSlider = new HorizontalSlider();
            MusicSlider.GridColumn = 1;
            MusicSlider.Value = AssetManager.defaultMusicVolume * 100f;
            MusicSlider.ValueChanged += (s, a) =>
            {
                AssetManager.defaultMusicVolume = MusicSlider.Value/100f;
                AssetManager.UpdateMusicVolume();
            };
            Music.Widgets.Add(MusicSlider);

            MyraMain.StandardAlignment(grid, Music);


            var returnToMain = new TextButton();
            returnToMain.Text = "Back";
            returnToMain.Click += (s, a) =>
            {
                MyraMain.SwitchMenu(MenuType.Settings);
            };
            MyraMain.StandardAlignment(grid, returnToMain);


        }
    }
}
