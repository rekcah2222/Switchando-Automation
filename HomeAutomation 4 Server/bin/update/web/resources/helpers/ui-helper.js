function loadClients() {
	var server = new Switchando();
	var clients = server.sendCommand("get/clients");

	var parsed = JSON.parse(clients);
	if (typeof parsed.status !== 'undefined' && parsed.status !== null) {
		window.location.replace("server-error.html?json=" + clients);
	} else {
		for (index = 0; index < parsed.length; ++index) {
			var x = document.getElementById("client_select");
			var option = document.createElement("option");
			option.text = parsed[index].Name;
			x.add(option);
		}
	}
}
function loadRooms() {
	var server = new Switchando();
	var clients = server.sendCommand("get/rooms");
	var parsed = JSON.parse(clients);
	if (typeof parsed.status !== 'undefined' && parsed.status !== null) {
		window.location.replace("server-error.html?json=" + clients);
	} else {
		for (index = 0; index < parsed.length; ++index) {
			var x = document.getElementById("room_select");
			var option = document.createElement("option");
			option.text = parsed[index].Name;
			x.add(option);
		}
	}
}
function loadButtons() {
	var server = new Switchando();
	var clients = server.sendCommand("get/devices");
	var parsed = JSON.parse(clients);
	if (typeof parsed.status !== 'undefined' && parsed.status !== null) {
		window.location.replace("server-error.html?json=" + clients);
	} else {
		for (index = 0; index < parsed.length; ++index) {
			if (parsed[index].ObjectType == "BUTTON") {
				var x = document.getElementById("buttons_select");
				var option = document.createElement("option");
				option.text = parsed[index].Name;
				x.add(option);
			}
		}
	}
}
function loadDevices() {
	var server = new Switchando();
	var clients = server.sendCommand("get/devices");
	var parsed = JSON.parse(clients);
	if (typeof parsed.status !== 'undefined' && parsed.status !== null) {
		window.location.replace("server-error.html?json=" + clients);
	} else {
		for (index = 0; index < parsed.length; ++index) {
			var x = document.getElementById("devices_select");
			var option = document.createElement("option");
			option.text = parsed[index].Name;
			x.add(option);
		}
	}
}