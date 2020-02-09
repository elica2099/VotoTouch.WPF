using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace VotoTouch.WPF
{
    public class CVotoImages
    {
        private const string IMG_EXT = ".png";
        private Window MainForm;
        private Image MainImage;
        private Grid MainGrid;

        public CVotoImages(Window AMainForm, Image AMainImage, Grid AMainGrid)
        {
            // costruttore
            MainForm = AMainForm;
            MainImage = AMainImage;
            MainGrid = AMainGrid;
        }

        // carico delle immagini nella finestra -----------------------------------------------------------------

        public void LoadImages(string AImage)
        {
            // cancello l'immagine prima perché sennò aumenta la memoria a palla;
            // non serve in WPF
            //if (MainForm.BackgroundImage != null)
            //    MainForm.BackgroundImage.Dispose();

            // prima la cerco nella cartella data
            if (!System.IO.File.Exists(VTConfig.Img_Path + AImage + IMG_EXT)) return;
            //MainForm.Background = new ImageBrush(new BitmapImage(new Uri(VTConfig.Img_Path + AImage + IMG_EXT)));

            var animation = new DoubleAnimation
            {
                From = 1, 
                To = 0.3,
                Duration = TimeSpan.FromMilliseconds(700),
                FillBehavior = FillBehavior.HoldEnd
            };
            //animation.Completed += (s, a) => MainImage.Opacity = 0;
            MainGrid.BeginAnimation(UIElement.OpacityProperty, animation);

            MainImage.Source = new BitmapImage(new Uri(VTConfig.Img_Path + AImage + IMG_EXT));
            var animation2 = new DoubleAnimation
            {
                From = 0.3, 
                To = 1,
                Duration = TimeSpan.FromMilliseconds(700),
                FillBehavior = FillBehavior.HoldEnd
            };
            MainGrid.BeginAnimation(UIElement.OpacityProperty, animation2);

            return;
        }

        // Check delle immagini nella finestra -----------------------------------------------------------------

        public bool CheckImageFolder()
        {
            // controllo se sono in locale, in tal caso non faccio nulla
            if (VTConfig.Img_Path.Contains(VSDecl.IMG_PATH_LOC))
                return false;

            try
            {
                // ora controllo che la cartella delle immagini sia vuota, se sì le copio tutte
                if (!Directory.EnumerateFileSystemEntries(VTConfig.Img_Path).Any())
                {
                    return CopyImages();
                }
                else
                {
                    // devo testare se sono uguali alla source ed eventualmente copiarle
                    string SourceImgPath = VTConfig.Exe_Path + VSDecl.SOURCE_IMG_PATH;
                    // testo se la cartella sul server è vuota (esempio del mio portatile)
                    if (!System.IO.Directory.Exists(SourceImgPath)) return false;
                    // carico tutti i files
                    string[] fileEntries = System.IO.Directory.GetFiles(SourceImgPath);
                    foreach (string fileName in fileEntries)
                    {
                        // verifico se c'è nel corrispondente path
                        string dstName = VTConfig.Img_Path + System.IO.Path.GetFileName(fileName);
                        if (System.IO.File.Exists(dstName))
                        {
                            // devo testare se sono uguale
                            FileInfo src_f = new FileInfo(fileName);
                            FileInfo dst_f = new FileInfo(dstName);
                            // testo se sono diversi come data di modifica
                            DateTime s_time = src_f.LastWriteTime;
                            DateTime d_time = dst_f.LastWriteTime;
                            if (d_time < s_time)
                                System.IO.File.Copy(fileName, dstName, true);
                        }
                        else
                            // copio il file
                            System.IO.File.Copy(fileName, dstName, true);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        // -----------------------------------------------------------------
        // copia delle immagini nella cartella Data
        // -----------------------------------------------------------------

        public bool CopyImages()
        {
            try
            {
                // mi trovo il path dell'eseguibile
                string SourceImgPath = VTConfig.Exe_Path + VSDecl.SOURCE_IMG_PATH;
                // allora, devo testare se c'è la cartella data in locale, se non c'è devo crearla
                // e copiare le immagini
                if (!System.IO.Directory.Exists(VTConfig.Img_Path) && System.IO.Directory.Exists(SourceImgPath))
                {
                    // creo la sotto cartella
                    System.IO.Directory.CreateDirectory(VTConfig.Img_Path);
                }
                // Process the list of files found in the directory.
                string[] fileEntries = System.IO.Directory.GetFiles(SourceImgPath);
                foreach (string fileName in fileEntries)
                {
                    string dstName = VTConfig.Img_Path + System.IO.Path.GetFileName(fileName);
                    System.IO.File.Copy(fileName, dstName, true);
                }

                // ok, tutto a posto
                return fileEntries.Length != 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }
    }
}