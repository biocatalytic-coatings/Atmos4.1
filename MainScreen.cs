using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Dynamic;
using System.Configuration;

namespace AtMoS3
{
    public partial class MainScreen : Form
    {
        private string version;

        public MainScreen()
        {
            InitializeComponent();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //  Displays the About form.
            About myNewForm = new About();
            myNewForm.Show();
        }

#pragma warning disable IDE1006 // Naming Styles
        private void Form1_Load(object sender, EventArgs e)
#pragma warning restore IDE1006 // Naming Styles
        {
            //this.ControlBox = false;
            //blankLabels();
            lblStatus.Text = "Awaiting start.";
            //backgroundWorker1.RunWorkerAsync();
            bwGetSystemTime.RunWorkerAsync();

            //  We set a version here so that the features of the program can be configured depending on
            //  which organisation is using the program.  The active features are determined by the value 
            //  of "User" in the app.config file..

            version = ConfigurationManager.AppSettings["User"];
            configVersion();

    }

        private void configVersion()
        {
            //  Here we make changes to the program features at runtime depending on which organisation
            //  is using the program.  The active features are determined by the value 
            //  of "User" in the app.config file.
            if (version == "Southern Cross University")
            {
                this.Text = "AtMoS - SCU";
                tabControl1.TabPages.Remove(Sample);
            }
            else
            {
                this.Text = "AtMoS - Illawarra Coatings";
                toolStripStatusLabel1.Text = "AtMoS - Licensed to Illawarra Coatings.";
            }

            //  This code simply removes the label text used during design.
            lblSystemTime.Text = "";
            lblTemperature.Text = "";
            lblHumidity.Text = "";
            lblPressure.Text = "";
            lblNOAE.Text = "";
            lblNOWE.Text = "";
            lblDataFileLocation.Text = "";
            lblNO2AE.Text = "";
            lblNO2WE.Text = "";
            lblNOConc.Text = "";
            lblNO2Conc.Text = "";

        }

        private void closingEvent(object sender, FormClosingEventArgs e)
        {

            System.Windows.Forms.Application.Exit();
            Environment.Exit(0);
            this.Close();
        }
        
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //  This section of code creates a new datafile and writes its location to a label so other parts of the
            //  program can reference the file location and write data.  The code also now loads column heading info
            //  into the datafile.
            //  Code works correctly...
            Stream _myStream;
            try
            {
                if (txtExpDesc.Text == "")
                {
                    MessageBox.Show("I'd prefer that you enter a description for the experiment before continuing.", "Oops...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
                }
                else
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.ShowDialog();
                    _myStream = saveFileDialog1.OpenFile();
                    lblDataFileLocation.Text = saveFileDialog1.FileName;
                    _myStream.Close();

                    //  Now write the datafile name, Experiment description and Column headings to the datafile.
                    //  Append the .csv extension to the Experiment number to generate the filename.
                    string logFileName = lblDataFileLocation.Text + ".csv";

                    //  Here we construct the datastrings.  The \r ending is important to ensure that each data entry begins on a new line.
                    string expDesc = txtExpDesc.Text + "\r";
                    string dataHeadings = "Time stamp" + "," + "Atmospheric Pressure" + "," + "Temperature" + "," + "Humidity" + "," + "NOAE Volts" + "," + "NOWE Volts" + "," + "NO2AE Volts" + "," + "NO2WE Volts" + "\r";

                    // Write the datastring to the file "_logFileName".
                    using (StreamWriter outputFile = File.AppendText(logFileName))
                        try
                        {
                            outputFile.WriteLine(expDesc);
                            outputFile.WriteLine(dataHeadings);
                            outputFile.Close();
                        }
                        catch
                        {

                        }
                }
            }
            catch
            {
            }
            finally
            {
                //  Once the datafile has been created we make a number of menu items visible.  This prevents a user from
                //  trying to start the program when the datafile has not been created.
                aquisitionToolStripMenuItem.Visible = true;

            }
        }

        private void write2DataFile()
        {
            //  This is the method we use to write data to the datafile.
            //  Format the timestamp so that date and time are within the same cell when loaded into Excel.
            string timeStamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            //  Append the .csv extension to the Experiment number to generate the filename.
            string _logFileName = lblDataFileLocation.Text + ".csv";

            //  Here we construct the datastring.  The \r ending is important to ensure that each data entry begins on a new line.
            string _data2Write = timeStamp + "," + lblPressure.Text + "," + lblTemperature.Text + "," + lblHumidity.Text + "," + lblNOAE.Text + "," + lblNOWE.Text + "," + lblNO2AE.Text + "," + lblNO2WE.Text + "," + "\r";

            // Write the datastring to the file "_logFileName".
            using (StreamWriter outputFile = File.AppendText(_logFileName))
                try
                {
                    outputFile.WriteLine(_data2Write);
                    outputFile.Close();
                }
                catch
                {
                }
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //  Let's first of all make certain that there is a datafile created to store experiment output.
            try
            {
                if (lblDataFileLocation.Text == "")
                {
                    MessageBox.Show("You need to have created a datafile before you can start data aquisition.", "Oops...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
                }
            }
            catch
            {
            }

        }


        private void setlblStatusTextSafely(string text)
        {
            //  We use the InvokeRequired method to prevent a  "Cross thread operation not valid".  This error occurs when we try to 
            //  call a Windows Forms control from a thread that didn't create that control.  We can pass a text value from the calling 
            //  function.
            //  
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new MethodInvoker(delegate { lblStatus.Text = text; }));
            }
            else
            {
                lblStatus.Text = text;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {

            System.Windows.Forms.Application.Exit();
            Environment.Exit(0);
            this.Close();
        }

        private void bwGetClimate_DoWork(object sender, DoWorkEventArgs e)
        {
            /*  bwGetClimate is used to update the climate information on the form.  This is now handled by the bwgetClimate_DoWork
             *  function using the runPythonScript() function rather than the previous dedicated getClimate(). 
             *  This thread is started when data aquisition is commenced.
             */  

            while (true)
            {
                int delayTime = Convert.ToInt32(txtClimateUpdatedInterval.Text) * 1000;
                DateTime finishTime = (DateTime.Now).AddMilliseconds(delayTime);

                string myProgram = "Adafruit_BME280_Library/examples/mybme280";
                string programType = "climate";
                runPythonScript(myProgram, 4, 0, "1", programType);

                //This is the loop described above that creates the delay similiar to Thread.Sleep().
                while (DateTime.Now < finishTime)
                {
                    //  Create a loop
                }
            }
                
        }


        private void bwGetSystemTime_DoWork(object sender, DoWorkEventArgs e)
        {
            //  Backgroundworker 2 is used to update the system time on the form.  System time is used both as a check that the 
            //  program has not been caught in an unresponsive loop but also as the source of the timestamp information for
            //  writing to the datafile.  The timestamp for publishing to the cloud is derived from the called python script
            //
            //  We use the InvokeRequired method to prevent a  "Cross thread operation not valid".This error occurs when we try to
            //  call a Windows Forms control from a thread that didn't create that control.  

            //  This thread is started at form_load and doesn't have a stop function.
            while (true)
            {
                DateTime nextSystemTimeUpdate = (DateTime.Now).AddMilliseconds(100);
                lblSystemTime.Invoke(new MethodInvoker(delegate { lblSystemTime.Text = DateTime.Now.ToString(); }));
                while (DateTime.Now < nextSystemTimeUpdate)
                {
                    //  Create a loop
                }
            }            
        }
                  


        /*
        private void electrodeOffsetMeasurementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bwCalculateElectrodeOffsets.RunWorkerAsync();
        }

        

        private void electrodeOffsetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bwCalculateElectrodeOffsets.RunWorkerAsync();
        }

        private void bwCalculateElectrodeOffsets_DoWork(object sender, DoWorkEventArgs e)
        {
            int count = 0;
            double sumNOWE = 0;
            double sumNOAE = 0;
            double sumNO2WE = 0;
            double sumNO2AE = 0;
            double avgNOWE = 0;
            double avgNOAE = 0;
            double avgNO2WE = 0;
            double avgNO2AE = 0;


            setlblStatusTextSafely("Calculating electrode offsets.");

            startPump();
            while (true)
            {

                DateTime finishTimeBW5 = (DateTime.Now).AddMilliseconds(2000);
                getGasContinuous();
                sumNOWE += Convert.ToDouble(lblNOWE.Text);
                sumNOAE += Convert.ToDouble(lblNOAE.Text);
                sumNO2WE += Convert.ToDouble(lblNO2WE.Text);
                sumNO2AE += Convert.ToDouble(lblNO2AE.Text);

                count += 1;

                avgNOWE = Math.Round((sumNOWE / count),5);
                avgNOAE = Math.Round((sumNOAE / count),5);
                avgNO2WE = Math.Round((sumNO2WE / count),5);
                avgNO2AE = Math.Round((sumNO2AE / count),5);
                

                if (txtNOWEOffset.InvokeRequired)
                {
                    txtNOWEOffset.Invoke(new MethodInvoker(delegate { txtNOWEOffset.Text = avgNOWE.ToString("#.#####"); }));
                    txtNOAEOffset.Invoke(new MethodInvoker(delegate { txtNOAEOffset.Text = avgNOAE.ToString("#.#####"); }));
                    txtNO2WEOffset.Invoke(new MethodInvoker(delegate { txtNO2WEOffset.Text = avgNO2WE.ToString("#.#####"); }));
                    txtNO2AEOffset.Invoke(new MethodInvoker(delegate { txtNO2AEOffset.Text = avgNO2AE.ToString("#.#####"); }));
                }
                else
                {
                    txtNOWEOffset.Text = txtNOWEOffset.Text = avgNOWE.ToString("#.#####");
                    txtNOAEOffset.Text = txtNOAEOffset.Text = avgNOAE.ToString("#.#####");
                    txtNO2WEOffset.Text = txtNO2WEOffset.Text = avgNO2WE.ToString("#.#####");
                    txtNO2AEOffset.Text = txtNO2AEOffset.Text = avgNO2AE.ToString("#.#####");
                }

                //publishData();
                write2DataFile();

                while (DateTime.Now < finishTimeBW5)
                {
                    //  Create a loop
                }
            }

        }
                */

        /*
        private void getGasContinuous()
        {
            /*  This getGas() method calls a python script that reads electrode values from the South Coast Science
             *  DFE which has an series of Alphasense electrochemical sensors attached to an AFE.
             *  
             *  The script takes one reading per second (1Hz) for a period of time determined by the value of
             *  txtSamplingTime.Text.  A delay is built into the script to force the 1Hz measurement period.
             *  
             *  Once the sampling is completed, the script calculates the average value for the electrode outputs
             *  and returns that value to this program.
             *  
             *  There were some issues with running this program initially.  When the version of python was 
             *  changed to python3, the error disappeared.
             *  
            

            string python = @"/usr/bin/python3";
            string args = @"/home/pi/Adafruit_Python_ADS1x15/Gas2.py";

            try
            {
                Process getgas = new Process();
                ProcessStartInfo publishProcessStartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    FileName = python,
                    Arguments = args
                };

                getgas.StartInfo = publishProcessStartInfo;
                getgas.Start();
                //getgas.WaitForExit();

                StreamReader _myStreamReader = getgas.StandardOutput;
                string NO_WE = _myStreamReader.ReadLine();
                string NO_AE = _myStreamReader.ReadLine();
                string NO2_WE = _myStreamReader.ReadLine();
                string NO2_AE = _myStreamReader.ReadLine();

                //  We use the InvokeRequired method to prevent a  "Cross thread operation not valid".This error occurs when we try to
                //  call a Windows Forms control from a thread that didn't create that control.  

                lblNOWE.Invoke(new MethodInvoker(delegate { lblNOWE.Text = NO_WE; }));
                lblNOAE.Invoke(new MethodInvoker(delegate { lblNOAE.Text = NO_AE; }));
                lblNO2WE.Invoke(new MethodInvoker(delegate { lblNO2WE.Text = NO2_WE; }));
                lblNO2AE.Invoke(new MethodInvoker(delegate { lblNO2AE.Text = NO2_AE; }));
            }
            catch
            {                
            }

        }
*/

        /*
        private void publishElectrodeValues(string objNOWE, string NO_WE, string objNOAE, string NO_AE, string objNO2WE, string NO2_WE, string objNO2AE, string NO2_AE)
        {
            /*  This method publishes the electrode values to the form.
             *  We use the InvokeRequired method to prevent a "Cross thread operation not valid".This error occurs when we try to
             *  call a Windows Forms control from a thread that didn't create that control.  We can pass a text value from the calling
             *  function using this method.
             * 
             *  We do a test to determine if InvokeRequired is required for the first of the objects to update.  If it is required
             *  then it will be required for all the objects being updated.  This saves multiple if/else conditions.                
            

            if (lblNOWE.InvokeRequired)
            {
                lblNOWE.Invoke(new MethodInvoker(delegate { objNOWE = NO_WE; }));
                lblNOAE.Invoke(new MethodInvoker(delegate { objNOAE = NO_AE; }));
                lblNO2WE.Invoke(new MethodInvoker(delegate { objNO2WE = NO2_WE; }));
                lblNO2AE.Invoke(new MethodInvoker(delegate { objNO2AE = NO2_AE; }));
            }
            else
            {
                objNOWE = NO_WE;
                objNOAE = NO_AE;
                objNO2WE = NO2_WE;
                objNO2AE = NO2_AE;
            }
                       
        }
            */

        /*
        private void startBaselineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bwCalculateElectrodeOffsets.RunWorkerAsync();
        }
        */
        

        private void publish2Adafruit()
        {
            //  Lets publish the climate data to Adafruit just to check and see if the python script works correctly.

            //  Yes...this is working correctly.  

            string python = @"/usr/bin/python3";
            string args3 = string.Format(@"/home/pi/Programs/Python/publish2Cloud/publish2Cloud.py {0} {1} {2} {3} {4} {5} {6}", lblTemperature.Text, lblHumidity.Text, lblPressure.Text, lblNOAE.Text, lblNOWE.Text, lblNO2AE.Text, lblNO2WE.Text);

            try
            {
                Process publish = new Process();
                ProcessStartInfo publishProcessStartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    FileName = python,
                    Arguments = args3
                };

                publish.StartInfo = publishProcessStartInfo;
                publish.Start();
                publish.WaitForExit();
            }
            catch
            {
            }
        }



        private void runPythonScript(string fileName, int myPin, int gpioState, string samplingTime, string programType)
        {
            /*  This function is currently under development.  It's purpose is to combine all the other functions that call python 
             *  scripts to energise or deenergise relays into one main piece of code.
            */

            /*  Define where the python complier is located and which script we are going to run.  All the scripts needed for the 
             *  operation of the program are now stored in the /home/pi/Programs/pythonScripts/ folder.
            */
            string python = @"/usr/bin/python3";
            string runPythonScript = string.Format(@"/home/pi/" + fileName + ".py {0} {1} {2} {3} {4}", fileName, myPin, gpioState, samplingTime, programType);

            try
            {
                Process pythonScript = new Process();
                ProcessStartInfo pythonScriptStartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    FileName = python,
                    Arguments = runPythonScript
                };

                pythonScript.StartInfo = pythonScriptStartInfo;
                pythonScript.Start();

                switch (programType)
                {
                    case ("gas"):
                        StreamReader _myStreamReader = pythonScript.StandardOutput;
                        string NO_WE = _myStreamReader.ReadLine();
                        string NO_AE = _myStreamReader.ReadLine();
                        string NO2_WE = _myStreamReader.ReadLine();
                        string NO2_AE = _myStreamReader.ReadLine();


                        lblNOWE.Invoke(new MethodInvoker(delegate { lblNOWE.Text = NO_WE; }));
                        lblNOAE.Invoke(new MethodInvoker(delegate { lblNOAE.Text = NO_AE; }));
                        lblNO2WE.Invoke(new MethodInvoker(delegate { lblNO2WE.Text = NO2_WE; }));
                        lblNO2AE.Invoke(new MethodInvoker(delegate { lblNO2AE.Text = NO2_AE; }));
                        break;
                    case ("climate"):
                        StreamReader _myStreamReader2 = pythonScript.StandardOutput;
                        string _temp = _myStreamReader2.ReadLine();
                        string _press = _myStreamReader2.ReadLine();
                        string _humid = _myStreamReader2.ReadLine();

                        lblTemperature.Invoke(new MethodInvoker(delegate { lblTemperature.Text = _temp; }));
                        lblPressure.Invoke(new MethodInvoker(delegate { lblPressure.Text = _press; }));
                        lblHumidity.Invoke(new MethodInvoker(delegate { lblHumidity.Text = _humid; }));
                        break;
                    default:
                        break;
                }
            }
            catch
            {
            }
           
            //  TO DO - Now change the call for the calibration hood solenoid tom use this function.
        }

        private void bwPublishContinuous_DoWork(object sender, DoWorkEventArgs e)
        {
            /*  Add an initial delay here to ensure that data is available on the form to be written to Adafruit.
             *  Without this delay, the first publish thgrows an error that a list item is out of range in the 
             *  publish2Cloud.py script.
             */
            Thread.Sleep(5000);
 
            while (true)
            {
                DateTime nextUpdateTime = (DateTime.Now).AddMilliseconds(15000);
                publish2Adafruit();

                //This is the loop that creates the delay similiar to Thread.Sleep().
                while (DateTime.Now < nextUpdateTime)
                {
                    //  Create a loop
                }

            }
        }

        private void newContinuousToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bwGetClimate.RunWorkerAsync();
            bwGasCont.RunWorkerAsync();
        }

        

        private void bwGasCont_DoWork(object sender, DoWorkEventArgs e)
        {
            bwPublishContinuous.RunWorkerAsync();
            string fileName = "Adafruit_Python_ADS1x15/myGas";
            string programType = "gas";
            while (true)
            {
                DateTime finishTime = (DateTime.Now).AddMilliseconds(1000);

                runPythonScript(fileName, 5, 1, "1", programType);
                write2DataFile();

                while (DateTime.Now < finishTime)
                {
                    // create a loop
                }
            }
        }


        private void bwgasPulsed_DoWork(object sender, DoWorkEventArgs e)
        {
            // 09/01/2021 1045 - Added this bw to return pulsed gas to stand alone code to fix adafruit update issue.


            while (true)
            {
                DateTime finishTimeBW3 = (DateTime.Now).AddMilliseconds(Convert.ToInt32(txtSleepTime.Text) * 1000);
                DateTime purgeFinish = (DateTime.Now).AddMilliseconds(Convert.ToInt32(txtPurgeTime.Text) * 1000 + 1000);

                // Energise and open the usb pump solenoid valve.
                string openSolenoid = "Programs/pythonScripts/relayState";
                string relay = "relay";
                runPythonScript(openSolenoid, 26, 0, "1", relay);

                DateTime pumpStartDelay = (DateTime.Now).AddMilliseconds(1000);
                setlblStatusTextSafely("Sensor purge cycle started.");
                while (DateTime.Now < pumpStartDelay)
                {
                    //  Create a loop
                }

                // Start the usb pump
                string startPump = "Programs/pythonScripts/relayState";
                runPythonScript(startPump, 4, 0, "1", relay);

                while (DateTime.Now < purgeFinish)
                {
                    //  Loop
                }

                setlblStatusTextSafely("Analysing chamber atmospheric composition");

                // Start the getGas.py program
                string fileName = "Adafruit_Python_ADS1x15/myGas";
                string gas = "gas";
                string samplingTime = txtSamplingTime.Text;
                runPythonScript(fileName, 5, 1, samplingTime, gas);

                // Stop the usb pump
                string stopPump = "Programs/pythonScripts/relayState";
                runPythonScript(stopPump, 4, 1, "1", relay);
                DateTime pumpStopDelay = (DateTime.Now).AddMilliseconds(1000);
                while (DateTime.Now < pumpStopDelay)
                {
                    //  Create a loop
                }

                setlblStatusTextSafely("Sleeping...waiting for next cycle");

                // De-energise and close the usb pump solenoid valve.
                string closeSolenoid = "Programs/pythonScripts/relayState";
                runPythonScript(closeSolenoid, 26, 1, "1", relay);

                //Thread.Sleep(10000);
                publish2Adafruit();

                write2DataFile();

                //This is the loop described above that creates the delay similiar to Thread.Sleep().
                while (DateTime.Now < finishTimeBW3)
                {
                    //  Create a loop
                }
            }

            

        }

        private void pulsedStandaloneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bwGetClimate.RunWorkerAsync();
            bwgasPulsed.RunWorkerAsync();
        }

    }

    /*
     * THINGS TO DO.
     * 
     * Need to make sure that aquisition can't start without a datafile being cfeated.
     * 
     * Look at combining all the relay operations into one function and pass the filename, pin number and state as variables.
     * Change "pythonstartpump" to "changeState" in solenoidState(). --- COMPLETE
     * Look at combining the startPump() and stopPump() functions into one and pass the python script file as a variable. --- COMPLETE
     * Look at combining the openSolenoid() and closeSolenoid() functions into one and pass the python script file as a variable. --- COMPLETE
     * 
     */

    /*  atmos4.1
     *  
     *  09/01/2021 1255 - Major code purge completed.  All major functions now run from runPythonScript().
     *  07/01/2021 0000 - Create relayAction function to manage all python script calls.
     *  06/01/2021 2235 - Create single function to open or close the gas hood solenoid valve.
     *  06/01/2021 1819 - Remove all the if statements in the getGasContinuous function as they are not required.
     *  05/01/2021 1319 - Corrected Adafruit publish on continuous measurement.
     *  05/01/2021 1126 - Create bw to publish continuous measurements.
     *  05/01/2021 1054 - Change pulse sampling purge, measure and sleep times.
     *  04/01/2021 1546 - Remove additional delay in bw3 finishTime.
     *  04/01/2021 1532 - Increase finish times for purge and sampling in getGasPulsed to account for solenoid delay.
     *  04/01/2021 1238 - Add gas hood solenoid energised advisory...reduce delay to 1 second.
     *  04/01/2021 1227 - delayLoop does not work so add directly to getGasPulsed function.
     *  04/01/2021 1219 - Use delayLoop in getGasPulsed to create delay between solenoid opening/closing and pump starting/stopping.
     *  
     *  */




}
