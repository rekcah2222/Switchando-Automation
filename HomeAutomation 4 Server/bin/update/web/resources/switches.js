var protocol = "";
var host = "";
var login = "";
var firstRun = true;
var server;

function onStartup() {

	server = new Switchando();

    /*if (get('fs') != null) {
        document.getElementById('fsbtn').style.display = 'block';
    }*/
	var data = "";
	try {
		data = server.sendCommand("/get/rooms", "/auth.html", "auth.html?data=Wrong username and / or password");
	} catch(ex) {
		window.location = "/auth.html";
		return;
	}
	if (data.includes("{\"status\":")) {
		var parsed = JSON.parse(data);
		window.location = "/auth.html?data=" + parsed.description;
		return;
	}
	onLoad(data);
}
function onLoad(json)
{
    if (firstRun == false)
    {
        updateSwitches(json);
        return;
    }
    var parsed = JSON.parse(json);
    var innerHtml = "";
    //document.getElementById('roomname').innerHTML = parsed.room;

    for (index = 0; index < parsed.length; ++index) {

	    innerHtml += "<br /><br />";
	    if (parsed[index].Hidden == true) continue;

	    var roomWidget = "";
	    var mainSwitch = false;

	    var objectsInnerHtml = "";
        for (objectIndex = 0; objectIndex < parsed[index].Objects.length; ++objectIndex)
        {
            var fragment = loadFile("/switchando-fragments/" + parsed[index].Objects[objectIndex].ObjectModel);
            fragment = replaceAll(fragment, "%name%", parsed[index].Objects[objectIndex].Name);
            fragment = replaceAll(fragment, "%desc%", parsed[index].Objects[objectIndex].Description);
            fragment = replaceAll(fragment, "%protocol%", protocol);
            fragment = replaceAll(fragment, "%host%", host);
            fragment = replaceAll(fragment, "%value%", parsed[index].Objects[objectIndex].Value);
            fragment = replaceAll(fragment, "%value_r%", parsed[index].Objects[objectIndex].ValueR);
            fragment = replaceAll(fragment, "%value_g%", parsed[index].Objects[objectIndex].ValueG);
            fragment = replaceAll(fragment, "%value_b%", parsed[index].Objects[objectIndex].ValueB);
            fragment = replaceAll(fragment, "%password%", get('password'));
            objectsInnerHtml += fragment;

            /*if (parsed[index].Objects[objectIndex].ObjectModel == "SWITCH")
            {
                /*objectsInnerHtml += '<div id="div-' + parsed[index].Objects[objectIndex].Name + '" style="width:100%;position:relative;background-color:white" class="mdl-shadow--2dp switchdiv">' +
                    '<h4 style="color:black">' + parsed[index].Objects[objectIndex].Name + '</h4>' +
                    '<div style="position:absolute;right:15px;top:10px">' +
                    '<label style="height:100%" class="mdl-switch mdl-js-switch mdl-js-ripple-effect" for="switch-' + parsed[index].Objects[objectIndex].Name + '">';
                if (parsed[index].Objects[objectIndex].Switch == false)
                {
                    objectsInnerHtml += '<input type="checkbox" id="switch-' + parsed[index].Objects[objectIndex].Name + '" class="mdl-switch__input" onClick="onClick(&quot;' + parsed[index].Objects[objectIndex].Name + '&quot;)" />';
                }
                else
                {
                    mainSwitch = true;
                    objectsInnerHtml += '<input type="checkbox" id="switch-' + parsed[index].Objects[objectIndex].Name + '" class="mdl-switch__input" onClick="onClick(&quot;' + parsed[index].Objects[objectIndex].Name + '&quot;)" checked />';
                }
                objectsInnerHtml += '<span class="mdl-switch__label"></span>' +
                    '</label>' +
                    '</div>' +
                    '<p style="color:gray">' +
                    parsed[index].Objects[objectIndex].Description +
                    '</p>' +
                    '</div>';

                var fragment = loadFile("/switchando-fragments/SWITCH");
                fragment = replaceAll(fragment, "%name%", parsed[index].Objects[objectIndex].Name);
                fragment = replaceAll(fragment, "%desc%", parsed[index].Objects[objectIndex].Description);
                objectsInnerHtml += fragment;
            }
            if (parsed[index].Objects[objectIndex].ObjectModel == "BLINDS") {
                objectsInnerHtml += '<div id="div-' + parsed[index].Objects[objectIndex].Name + '" style="width:100%;position:relative;background-color:white" class="mdl-shadow--2dp switchdiv">' +
                    '<h4 style="color:black">' + parsed[index].Objects[objectIndex].Name + '</h4>' +
                    '<div style="position:absolute;right:15px;top:10px">';

                objectsInnerHtml += '<button onClick="switchOn(&quot;' + parsed[index].Objects[objectIndex].Name + '&quot;)" class="mdl-button mdl-js-button mdl-button--raised mdl-js-ripple-effect dashbutton"><i class="material-icons">arrow_upward</i></button><button onClick="switchOff(&quot;' + parsed[index].Objects[objectIndex].Name + '&quot;)" class="mdl-button mdl-js-button mdl-button--raised mdl-js-ripple-effect dashbutton"><i class="material-icons">arrow_downward</i></button>' +
                    '</div>' +
                    '<p style="color:gray">' +
                    parsed[index].Objects[objectIndex].Description +
                    '</p>' +
                    '</div>';
            }*/
        }

        roomWidget += '<div id="div-' + parsed[index].Name + '" style="width:100%;position:relative;" class="">' +
                    '<h4 style="color:black">' + parsed[index].Name + '</h4>' +
                    '<div style="position:absolute;right:15px;top:0px">' +
                    '<label style="height:100%" class="mdl-switch mdl-js-switch mdl-js-ripple-effect" for="switch-' + parsed[index].Name + '">';
        if (mainSwitch == false) {
            roomWidget += '<input type="checkbox" id="switch-' + parsed[index].Name + '" class="mdl-switch__input" onClick="onClick(&quot;' + parsed[index].Name + '&quot;)" />';
        }
        else {
            roomWidget += '<input type="checkbox" id="switch-' + parsed[index].Name + '" class="mdl-switch__input" onClick="onClick(&quot;' + parsed[index].Name + '&quot;)" checked />';
        }
        roomWidget += '<span class="mdl-switch__label"></span>' +
            '</label>' +
            '</div>' +
            '</div>';

        innerHtml += roomWidget;
        innerHtml += objectsInnerHtml;

    }

    document.getElementById('switches_div').innerHTML += innerHtml;
    collapse();
    firstRun = false;
    setInterval(function () {
		onLoad(server.sendCommand("/get/rooms"));
    }, 1000);

    $('#search').on('input', function () {
        var pageDivs = document.getElementsByClassName("switchdiv");
        if ($(this).val() == "")
        {
            for (i = 0; i < pageDivs.length; i++) {
                 pageDivs[i].style.display = "";
            }
            return;
        }
        for (i = 0; i < pageDivs.length; i++) {
            var id = pageDivs[i].id.substring(4).toLowerCase();
            if (id.includes($(this).val())) {
                pageDivs[i].style.display = "";
            } else pageDivs[i].style.display = "none";
        }
    });
    
    updateSwitches(json);
}

async function updateSwitches(json) {
    var pageDivs = await document.getElementsByClassName("mdl-switch__input");
    var parsed = await JSON.parse(json);

    for (index = 0; index < parsed.length; ++index) {
        if (parsed[index].Hidden == true) continue;
        var mainSwitch = false;
        for (objectIndex = 0; objectIndex < parsed[index].Objects.length; ++objectIndex) {
            if (parsed[index].Objects[objectIndex].Switch != undefined) {
                if (parsed[index].Objects[objectIndex].Switch == true) {
                    var myCheckbox = document.getElementById('switch-' + parsed[index].Objects[objectIndex].Name);
                    myCheckbox.parentElement.MaterialSwitch.on();
                    mainSwitch = true;
                }
                else
                {
                    var myCheckbox = document.getElementById('switch-' + parsed[index].Objects[objectIndex].Name);
                    myCheckbox.parentElement.MaterialSwitch.off();
                }
            }
        }
        if (mainSwitch) {
            var myCheckbox = document.getElementById('switch-' + parsed[index].Name);
            myCheckbox.parentElement.MaterialSwitch.on();
        }
        else {
            var myCheckbox = document.getElementById('switch-' + parsed[index].Name);
            myCheckbox.parentElement.MaterialSwitch.off();
        }
    }
}

function onClick(target)
{

	//var xhr = new XMLHttpRequest();
    if (document.getElementById('switch-' + target).checked)
    {
        //xhr.open('GET', protocol + '://' + host + "/api/id/" + target + "/switch?objname=" + target + "&switch=true" + "&password=" + get('password'), false);
		server.sendCommand("/id/" + target + "/switch?objname=" + target + "&switch=true");
    }
	else server.sendCommand("/id/" + target + "/switch?objname=" + target + "&switch=false");
    //xhr.send();
}
function switchOn(target) {
	server.sendCommand("/id/" + target + "/switch?objname=" + target + "&switch=true");
}
function switchOff(target) {
    /*var xhr = new XMLHttpRequest();
    xhr.open('GET', protocol + '://' + host + "/api/id/" + target + "/switch?objname=" + target + "&switch=false" + "&password=" + get('password'), false);
    xhr.send();*/
	server.sendCommand("/id/" + target + "/switch?objname=" + target + "&switch=false");
}

function get(name) {
    if (name = (new RegExp('[?&]' + encodeURIComponent(name) + '=([^&]*)')).exec(location.search))
        return decodeURIComponent(name[1]);
}
function show() {
document.getElementById('fsbtn').style.display = 'none';
	var elem = document.body;
        if ((document.fullScreenElement !== undefined && document.fullScreenElement === null) || (document.msFullscreenElement !== undefined && document.msFullscreenElement === null) || (document.mozFullScreen !== undefined && !document.mozFullScreen) || (document.webkitIsFullScreen !== undefined && !document.webkitIsFullScreen)) {
            if (elem.requestFullScreen) {
                elem.requestFullScreen();
            } else if (elem.mozRequestFullScreen) {
                elem.mozRequestFullScreen();
            } else if (elem.webkitRequestFullScreen) {
                elem.webkitRequestFullScreen(Element.ALLOW_KEYBOARD_INPUT);
            } else if (elem.msRequestFullscreen) {
                elem.msRequestFullscreen();
            }
        } else {
            if (document.cancelFullScreen) {
                document.cancelFullScreen();
            } else if (document.mozCancelFullScreen) {
                document.mozCancelFullScreen();
            } else if (document.webkitCancelFullScreen) {
                document.webkitCancelFullScreen();
            } else if (document.msExitFullscreen) {
                document.msExitFullscreen();
            }
        }
}
function collapse() {
    $(".option-content").hide();
    $(".arrow-up").hide();
    $(".option-heading").click(function () {
        $(this).next(".option-content").slideToggle(500);
        $(this).find(".arrow-up, .arrow-down").toggle(500);
    });
}
function loadFile(filePath) {
    var result = null;
    var xmlhttp = new XMLHttpRequest();
    xmlhttp.open("GET", filePath, false);
    xmlhttp.send();
    if (xmlhttp.status==200) {
        result = xmlhttp.responseText;
    }
    return result;
}
function replaceAll(str, find, replace) {
    return str.replace(new RegExp(find, 'g'), replace);
}