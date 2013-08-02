<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="dsService.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>DominoSpot</title>
    <link href='http://fonts.googleapis.com/css?family=Unkempt:700' rel='stylesheet' type='text/css'/>
    <link href="assets/microsoft/css/m-styles.min.css" rel="stylesheet"/>
    <link href="assets/config.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" method="post" runat="server">
        <div id="master">
           <div id="head">
               <div id="signin" style="float:right;">
                   <img alt="signin image" src="assets/Image/1367926539_sign-in.png" />
                   <a href="#" style="text-decoration:none;">Sign in</a>
               </div>
               <br /><br /><br />
               <h1 style="font-family: ‘Metrophobic’, Arial, serif; font-weight: 400;">Domino Spot</h1>               
           </div>
           <table id="main-content">
               <tr>
                   <td id="c-left"></td>
                   <td id="c-right">               
                        <div id="signup" class="m-input-prepend">
                            <input class="m-wrap" size="16" type="text" placeholder="First name"/>&nbsp;
                            <input class="m-wrap" size="16" type="text" placeholder="Last name"/>
                            <br />
                            <input id="email" class="m-wrap" size="16" type="text" placeholder="Email"/>
                            <br />
                            <input id="pass" class="m-wrap" size="16" type="password" placeholder="Password"/>
                            <label class="m-checkbox m-wrap">
                                <input type="checkbox" class="m-wrap" value=""/>
                                I agree <a id="agree" href="#">DominoSpot Terms</a>
                            </label>
                           <center><a id="suBtn" href="#" class="m-btn blue rnd">Sign up</a></center>
                        </div>           
                   </td>
               </tr>
           </table>
            
       </div>
        <div id="footer">

        </div>   
    </form>
</body>
</html>
