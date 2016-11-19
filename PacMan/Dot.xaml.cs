using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PacMan
{
    /// <summary>
    /// Interaction logic for Dot.xaml
    /// </summary>
    /// 

    public partial class Dot : UserControl
    {
        //Dot Type:  1=Regular, 2=PowerUp
        private int dotType;

        public Dot(int dt)
        {
            InitializeComponent();

            dotType = dt;

            if (dt == 2)
            {
                // Power up - change the default dot look
                // Small dot: X="-168" Y="-252" 
                // Large dot: x="-210" Y="-252"
                this.SpriteSheetOffset.X = -210;
                this.SpriteSheetOffset.Y = -252;
            }
        }
    }
}
