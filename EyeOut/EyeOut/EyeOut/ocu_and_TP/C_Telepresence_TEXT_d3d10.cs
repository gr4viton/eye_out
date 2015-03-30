using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Direct3D11;
using SharpOVR;

using System.Windows.Forms;


//using SharpDX.Direct2D1; // text
//using SharpDX.DirectWrite; // text
//using SharpDX.DXGI;


//using AlphaMode = SharpDX.Direct2D1.AlphaMode;
//using Factory = SharpDX.Direct2D1.Factory;

namespace EyeOut
{
    // Use these namespaces here to override SharpDX.Direct3D11
    /*
    using SharpDX.Toolkit;
    using SharpDX.Toolkit.Graphics;
    using SharpDX.DXGI;
    */

    /// <summary>
    /// EyeOut telepresence using SharpDX.Toolkit - text handling
    /// </summary>
    public partial class C_Telepresence : SharpDX.Toolkit.Game
    {
        
        //public Factory Factory2D { get; private set; }
        //public SharpDX.DirectWrite.Factory FactoryDWrite { get; private set; }
        //public WindowRenderTarget RenderTarget2D_text { get; private set; }
        //public SolidColorBrush SceneColorBrush { get; private set; }


        ///// <summary>
        ///// Return the Handle to display to.
        ///// </summary>
        //protected IntPtr DisplayHandle
        //{
        //    get
        //    {
        //        return ((System.Windows.Forms.Control)Window.NativeWindow).Handle;
        //    }
        //}

        //public void INIT_TP_text()
        //{

            
        //    Factory2D = new SharpDX.Direct2D1.Factory();
        //    FactoryDWrite = new SharpDX.DirectWrite.Factory();
            
            
        //    //HwndRenderTargetProperties properties = new HwndRenderTargetProperties();
        //    //properties.Hwnd = DisplayHandle;
        //    //properties.PixelSize = new SharpDX.Size2(renderTargetSize.Width, renderTargetSize.Height);
        //    //properties.PresentOptions = PresentOptions.None;

        //    //RenderTarget2D_text = new WindowRenderTarget(Factory2D, new RenderTargetProperties(new PixelFormat(Format.Unknown, AlphaMode.Premultiplied)), properties);
             
        //    //RenderTarget2D_text.AntialiasMode = AntialiasMode.PerPrimitive;

        //    //SceneColorBrush = new SolidColorBrush(RenderTarget2D_text, Color.White);

        //    FactoryDWrite = new SharpDX.DirectWrite.Factory();

        //    // Initialize a TextFormat
        //    TextFormat = new TextFormat(FactoryDWrite, "Gabriola", 96) { TextAlignment = TextAlignment.Center, ParagraphAlignment = ParagraphAlignment.Center };

        //    RenderTarget2D_text.TextAntialiasMode = TextAntialiasMode.Cleartype;

        //    ClientRectangle = new RectangleF(0, 0, renderTargetSize.Width, renderTargetSize.Height);

        //    SceneColorBrush.Color = Color.Black;       
        //     //*/
        //}
    }
}
