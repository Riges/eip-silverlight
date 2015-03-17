http://blogs.msdn.com/b/vsastuces/archive/2009/04/21/astuce-silverlight-comment-impl-menter-le-double-click-s026.aspx

DispatcherTimer _doubleClickTimer;_

Image _lastImage = null;_



public Page()

{

> InitializeComponent();



> _doubleClickTimer = new DispatcherTimer();_

> _doubleClickTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);_

> _doubleClickTimer.Tick += new EventHandler(DoubleClick\_Timer);_



> this.MouseLeftButtonDown += new MouseButtonEventHandler(Page\_MouseLeftButtonDown);

}



// too much time has passed for it to be a double click.

void DoubleClick\_Timer(object sender, EventArgs e)

{

> _doubleClickTimer.Stop();_

}

void Page\_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)


{


> if (_doubleClickTimer.IsEnabled)_


> {


> /// blabla

> }


> else


> {


> _doubleClickTimer.Start();_


> }