internal class Motor 
{
	public void Execute(Form sender, bool showWaitScreen)
	{
		BackgroundWorker worker = new BackgroundWorker();
		worker.RunWorkerCompleted += worker_RunWorkerCompleted;
		worker.DoWork += worker_DoWork;
		worker.RunWorkerAsync(backgroundActions);
	}
	
	private void worker_DoWork(object sender, DoWorkEventArgs e)
	{            
		e.Result = ExecuteActions(input);
	}
	
	private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		if (e.Error != null)
		{
			//ie Helpers.HandleCOMException(e.Error);
		}
		else
		{
			var results = e.Result as List<object>;
		}
	}
}		

internal class C_SPI 
{	
	private static object locker = new object();
	
	public static bool WriteData(ModbusCommand command, UInt16 address, int count, byte[] data)
	{
		lock (locker)
		{
			OpenConnection(null, null);
			WriteSerialPort(requestBuffer);
			responseBuffer = ReadSerialPort(8);
		}
	}
		
}

internal class Logger 
{
	private dataTable;
	private static Logger instance;
	public static Logger Instance
	{
		get
		{
			if(instance==null)
			{
				instance = new Logger();                    
			}
			return instance;
		}
	}		
			
	private Loger()
	{
		dataTable = new DataTable();
	}
	
	public DataTable Data {get{return dataTable;}}
}
