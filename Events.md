Dans "AccountFacebookLight" fichier AccountFacebook.cs

On déclare un type :

public delegate void OnGetComsCompleted(List

&lt;comment&gt;

 coms, string postId);

OnGetComsCompleted : nom du type
<br>List<br>
<br>
<comment><br>
<br>
 coms : premier paramètre<br>
<br>string postId : deuxième paramètre<br>
<br>
On met les parametres que l'on veut, ici des commentaires car c'est ce que l'on va récupérer, et aussi le postId pr savoir quel post cela va impacter.<br>
<br>
Ensuite on déclare un event du type que l'on vient de définir<br>
<br>
public event OnGetComsCompleted GetComsCalled;<br>
<br>
OnGetComsCompleted : type précédemment déclaré<br>
<br>GetComsCalled : Nom de l'event<br>
<br>
<br>
<br>
Ensuite du coté de l'affichage il faut s'abonner à cet Event pr définir une action lorsque l'on déclenchera l'event<br>
<br>
<br>
ex dans le userControl Coms :<br>
<br>
((AccountFacebookLight)Connexion.accounts[this.accountID]).GetComsCalled += new AccountFacebookLight.OnGetComsCompleted(Coms_GetComsCalled);<br>
<br>
<br>
Je récupère le compte sur lequel on travail qui est dans mon  objet "accounts" qui est ds la classe statique "Connexion" je le cast comme un objet "AccountFacebookLight" afin de pvr accéder à l'event (car ds accounts mon objet est sous la forme d'un AccountLight)<br>
<br>
Ensuite je m'abonne a cet event. (la méthode "Coms_GetComsCalled" sera appelé au déclenchement de l'"event")<br>
<br>
prototype de la fonction "Coms_GetComsCalled" avec les paremtre défini dans le type "OnGetComsCompleted" qui est le type de notre event :<br>
<br>void Coms_GetComsCalled(List<br>
<br>
<comment><br>
<br>
 coms, string postId)<br>
<br>
<br>
Puis retournons dans notre classe AccountFacebookLight et plus particulièrement dans le callback de la fonction ui nous intéresse, la ou sera déclencher l'event pr envoyer les données que l'on a recu.<br>
<br>
public void GetComsFQL_Completed(comments_get_response coms, object postId, FacebookException ex)<br>
{<br>
<blockquote>if (ex == null && coms.comment.Count > 0)<br>
{<br>
<blockquote>if (this.GetComsCalled != null)  //OBLIGATOIRE pr etre sur qu'il y a bien des abonnements sur l'event et éviter un plantage<br>
<blockquote>this.GetComsCalled.Invoke(coms.comment, postId.ToString());  // on déclenche l'event avec les bon parametre, en l'occurrence avec les données  que l'on vient de recevoir.<br>
</blockquote></blockquote>}<br>
}