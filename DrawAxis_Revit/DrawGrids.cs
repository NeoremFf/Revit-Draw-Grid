using System;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DrawGrids_Revit
{
    /// <summary>
    /// Interaction logic for plug-in
    /// 
    /// Get data from user from window in Revit
    /// Start Transaction
    /// Create Levels and Grids
    /// Set theyr position
    /// Commit Transaction and stop it
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class DrawGrids : IExternalCommand
    {
        enum TypeOfGrid
        {
            Vertical,
            Horizontal
        };

        private Autodesk.Revit.DB.Document doc;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get application and documnet objects
            UIApplication uiapp = commandData.Application;
            doc = uiapp.ActiveUIDocument.Document;


            // Show window where user can write data
            InputWin inputWin = new InputWin();
            inputWin.ShowDialog();

            try
            {
                // Get data
                int countOfVert = Convert.ToInt32(inputWin.textBox_CountOfVert.Text);
                int countOfHor = Convert.ToInt32(inputWin.textBox_CountOfHor.Text);
                int countOfLevels = Convert.ToInt32(inputWin.textBox_CountOfLevels.Text);

                double distanceOfVert = Convert.ToDouble(inputWin.textBox_DistanceOfVert.Text);
                double distanceOfHor = Convert.ToDouble(inputWin.textBox_DistanceOfHor.Text);
                double distanceOfLevels = Convert.ToDouble(inputWin.textBox_DistanceOfLevels.Text);

                double rangeOfVert = Convert.ToDouble(inputWin.textBox_RangeOfVert.Text);
                double rangeOfHor = Convert.ToDouble(inputWin.textBox_RangeOfHor.Text);
                double rangeOfLevels = Convert.ToDouble(inputWin.textBox_RangeOfLevels.Text);

                // Start Transaction
                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Create grid");

                    // Create Levels and Grids
                    CreateGrids(distanceOfVert, countOfVert, rangeOfVert, TypeOfGrid.Vertical);
                    CreateGrids(distanceOfHor, countOfHor, rangeOfHor, TypeOfGrid.Horizontal);
                    CreateLevels(distanceOfLevels, countOfLevels, rangeOfLevels);

                    t.Commit();
                }
            }
            catch (System.ArgumentException)
            {
                MessageBox.Show("Некорректный формат данных.");
                return Result.Failed;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message); 
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        /// <summary>
        /// Create Levels
        /// </summary>
        /// <param name="elevation"></param>
        /// <param name="count"></param>
        /// <param name="range"></param>
        /// <returns>Array of Level</returns>
        private Level[] CreateLevels(double elevation, int count, double range)
        {
            Level[] levels = new Level[count];
            // Begin to create a level
            double currentElevation = elevation;
            for (int i = 0; i < count; i++)
            {
                Level level = Level.Create(doc, currentElevation);
                if (null == level)
                {
                    throw new Exception("Не удалось создать уровень.");
                }
                currentElevation += elevation;
                levels[i] = level;
                level.Name = "Уровень " + (i + 2); // Change the level name
            }
            return levels;
        }

        /// <summary>
        /// Create Grids
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="count"></param>
        /// <param name="range"></param>
        /// <param name="type"></param>
        /// <returns>Array of Grid</returns>
        private Grid[] CreateGrids(double distance, int count, double range, TypeOfGrid type)
        {
            Grid[] grids = new Grid[count];
            XYZ start, end;
            char nameHor = 'A';
            // Begin to create a grid  
            if (type == TypeOfGrid.Vertical)
                start = new XYZ(0, 0, 0);
            else
                start = new XYZ(-distance, distance, 0);
            for (int i = 0; i < count; i++)
            {
                if (type == TypeOfGrid.Vertical)
                    end = new XYZ(start.X, start.Y + range, 0);
                else
                    end = new XYZ(start.X + range, start.Y, 0);
                Line line = Line.CreateBound(start, end);
                Grid grid = Grid.Create(doc, line);            
                if (grid == null)
                {
                    throw new Exception("Не удалось создать ось.");
                }

                if (type == TypeOfGrid.Vertical)
                {
                    start = new XYZ(start.X + distance, start.Y, 0);
                    grid.Name = (i + 1).ToString();
                }
                else
                {
                    start = new XYZ(start.X, start.Y + distance, 0);
                    grid.Name = (nameHor++).ToString();
                }
                
                grids[i] = grid;
            }
            return grids;
        }
    }
}
