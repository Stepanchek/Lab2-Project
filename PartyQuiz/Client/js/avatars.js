
var maleAvatars = ['Casual_Male.png','BlueSoldier_Male.png', 'Knight_Male.png', 'Suit_Male.png', 'Viking_Helmet_Male.png', 'Wizard_Male.png'];
var femaleAvatars = ['Casual_Female.png','Chef_Female.png', 'Goblin_Female.png', 'Kimono_Female.png', 'Suit_Female.png', 'Witch_Female.png'];
var avatars = {
    'male': maleAvatars,
    'female' : femaleAvatars};

var fadeDuration = 100;
var currentAvatarId = "none";
var currentGender;

function SetGender(gender){
    currentGender = gender;
    document.getElementById('avatar_group').hidden = false;
    NextAvatar();
}

function PrevAvatar()
{
 $( '#slideshow_image' ).fadeOut(fadeDuration, function()
 {
  var prev_val = document.getElementById( "img_no" ).value;
  var prev_val = Number(prev_val) - 1;
  if(prev_val <= -1)
  {
   prev_val = avatars[currentGender].length - 1;
  }
  currentAvatarId =  avatars[currentGender][prev_val];
  $( '#slideshow_image' ).attr( 'src' , 'avatars/' + currentGender + '/' + currentAvatarId);
  document.getElementById( "img_no" ).value = prev_val;
 });
 $( '#slideshow_image' ).fadeIn(fadeDuration);
}

function NextAvatar()
{
 $( '#slideshow_image' ).fadeOut(fadeDuration, function()
 {
  var next_val = document.getElementById( "img_no" ).value;
  var next_val = Number(next_val)+1;
  if(next_val >=  avatars[currentGender].length)
  {
   next_val = 0;
  }
  currentAvatarId =  avatars[currentGender][next_val];
  $( '#slideshow_image' ).attr( 'src' , 'avatars/' + currentGender + '/' + currentAvatarId );
  document.getElementById( "img_no" ).value = next_val;
 });
 $( '#slideshow_image' ).fadeIn(fadeDuration);
}