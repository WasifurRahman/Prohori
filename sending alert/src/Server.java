import java.net.*;
import java.awt.Toolkit;
import java.io.*;
public class Server {
	
	public static void main(String[] args){
		
		 int alarmCode=420;
		 ServerSocket echoServer = null;
	     int curCode;
	     DataInputStream is;
	     PrintStream os;
	     Socket clientSocket = null;
		
	     try {
	           echoServer = new ServerSocket(9999);
	        }
	        catch (IOException e) {
	           System.out.println(e);
	        } 
	     
	     
	     try {
	           clientSocket = echoServer.accept();
	           is = new DataInputStream(clientSocket.getInputStream());
	           os = new PrintStream(clientSocket.getOutputStream());
	// As long as we receive data, echo that data back to the client.
	           while (true) {
	             curCode = is.read();
	             if(curCode==alarmCode){
	            	 Toolkit.getDefaultToolkit().beep();
	             }
	             
	             //os.println(line); 
	           }
	        }   
	    catch (IOException e) {
	           System.out.println(e);
	        }
	    }
	}
	


