
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

document.getElementById("sendButton").disabled = true;
document.getElementById("callButton").disabled = true;

var toUser = null; 
var toConversation = null; 
var selectedUser = null; 

///////////////////////////////////////////////////////////////////////////////////////////////////////////
///// MESSAGE CHAT LOGIC
connection.on("IdentifyUser", function (user) { //
    currentUser = user;
}); //+
connection.on("RecieveOnlineUsers", function (userNames) { // метод отвечающий за пользователей которые находятся в онлайне
    Array.from(userNames); 
    var userNameWindow = document.getElementById("userOnlineList");
    const listItems = userNameWindow.querySelector('li');
    userNames.forEach(addOnlineUserNames); // каждый элемент массива обрабатывается методом
    const scrollingElement = document.getElementsByClassName("sidenavRight")[0]; 
    scrollingElement.scrollTop = scrollingElement.scrollHeight; // установка позиции прокрутки в самый низ
}); //+
function addOnlineUserNames(user) { // метод обработающий каждого пользователя который в онлайн и перекрасит кнопку в зеленый
    if (user.userIdentifier === currentUser.userIdentifier) { 
        return; 
    }
    var userNameWindow = document.getElementById("userOnlineList");
    const listItems = userNameWindow.querySelectorAll('li');
    for (let i = 0; i < listItems.length; i++)
    {
        var button = listItems[i].querySelector('button');
        if (user.name == button.textContent) {
            button.style.backgroundColor ='#03fc49';
        }
    }
} //+
connection.on("ReceiveContactList", function (contactList) { 
    Array.from(contactList);
    var userNameWindow = document.getElementById("userOnlineList");
    userNameWindow.innerHTML = "";
    contactList.forEach(addUserFromContact);
    const scrollingElement = document.getElementsByClassName("sidenavRight")[0];
    scrollingElement.scrollTop = scrollingElement.scrollHeight;
}); //+ загрузка списка контактов из БД
connection.on("LoadChat", function (messageList) {
    console.log("LoadChat")
    Array.from(messageList);
    messageList.forEach(addMessageFromDatabase);
}); // загрузка чата
function addMessageFromDatabase(message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    li.innerHTML = `<div class="one-message">
                        <p class="username"><b>ИМЯ</b></p>
                          <div class="message">
                             <p class="text-content"></p>
                          </div>
                        <span class="time-right">${message.sentTime}</span>
                    </div>`;
    li.getElementsByClassName("text-content")[0].textContent = message.messageText;
    const scrollingElement = document.getElementsByClassName("message-box")[0];
    scrollingElement.scrollTop = scrollingElement.scrollHeight;
}
function addUserFromContact(user) { 
    var li = document.createElement("li"); 
    var button = document.createElement("button");
    button.className = "user-disconnected"; 
    button.textContent = user.name; 
    button.onclick = function (event) { 
        if (toUser === user) {
            return;
        } 
        document.getElementById("chat-intro").innerHTML = `${user.name}`; 
        document.getElementById("sendButton").disabled = false
        document.getElementById("callButton").disabled = false;

        if (toConversation !== null) { 
     
            connection.invoke("LeaveRoom", toConversation); 
            toConversation = null;
        }
        if (toUser !== null && toUser !== user) { 
          
            connection.invoke("LeavePrivateChat", toUser.name); 
        }
        toUser = user; 
        document.getElementById("messagesList").innerHTML = ""; // зачистка сообщений
        connection.invoke("ConnectUserToConversation", currentUser, toUser); // подключение пользователя к частной переписке
       
    };
    li.appendChild(button); // элемент не упорядочного принимает кнопку
    document.getElementById("userOnlineList").appendChild(li);
} //+ // добавить пользователя из контакта по элементно ввиде кнопок
connection.on("SetCurrentConversation", function (conversationName) { 
    toConversation = conversationName;
});
connection.on("RecieveAvailableUsers", function (availableUsersList) { 
        var groupNameWindow = document.getElementById("userList");
        groupNameWindow.innerHTML = ""; 
        availableUsersList.forEach(addOnlineAvailableUsers);
        const scrollingElement = document.getElementsByClassName("groups-main")[0];
        scrollingElement.scrollTop = scrollingElemant.scrollHeight;  
}); 
function addOnlineAvailableUsers(user) { 
       
        var li = document.createElement("li"); 
        var button = document.createElement("button"); 
        
        button.className = "group-connected"; 
        button.innerHTML = `${user.userName}<br>${user.uniqueIdentifier}`;
    button.onclick = function (event) {  // каждую кнопку подписываем на событие
        selectedUser = user;
        var inputName = document.getElementById("GroupChatName");
        var uniqueId = document.getElementById("UniqueIdentifier");
        inputName.value = `${selectedUser.userName}`;
        uniqueId.value = `${selectedUser.uniqueIdentifier}`;
    };
        li.appendChild(button); 
        document.getElementById("userList").appendChild(li); 
} 
document.getElementById("addUserButton").addEventListener("click", function (event) { 
    var inputName = document.getElementById("GroupChatName");
    var uniqueId = document.getElementById("UniqueIdentifier");
    connection.invoke("AddNewUser", selectedUser.uniqueIdentifier);
    inputName.value = "";
    uniqueId.value = "";
    selectedUser = null;
}); //+ Событие в котором кнопка add добавляет добаыить пользователя в список контактов
document.getElementById("GroupChatName").addEventListener("input", function (event) { 
    const value = event.target.value;
    connection.invoke("UserSearchPattern",value);
}); //++ метод поиска пользователя из всех пользователей в базе данных по паттерну

document.getElementById("sendButton").addEventListener("click", function (event) { // подписываем кнопку на отправку сообщений
    var message = document.getElementById("messageInput").value;
    if (message !== "" && toUser !== null && toConversation !== null) {// избавься от toConversation
        connection.invoke("SendPrivateMessage", currentUser, toUser, message).catch(function (err) {
            return console.error(err.toString());
        });
        $("textarea").val("");
    }
    event.preventDefault();
});

connection.on("ReceiveMessage", function (user, message, timenow) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    li.innerHTML = `<div class="one-message ${currentUser.userIdentifier === user.userIdentifier ? "my-message" : ""}">
                    <p class="username"><b>${user.name}</b></p>
                    <div class="message">
                    <p class="text-content"></p>
                    </div>
                    <span class="time-right">${timenow}</span>
                    </div>`;
    li.getElementsByClassName("text-content")[0].textContent = message;
    const scrollingElement = document.getElementsByClassName("message-box")[0];
    scrollingElement.scrollTop = scrollingElement.scrollHeight;
});


///////////////////////////////////////////////////////////////////////////////////////////////////////////
///// VIDEO CHAT
const startButton = document.getElementById('btnConnect');
const hangupButton = document.getElementById('btnDisconnect');

const localVideo = document.getElementById('localVideo');
const remoteVideo = document.getElementById('remoteVideo');

let pc; // peer connection
let localStream;

const signaling = new BroadcastChannel('webrtc');
signaling.onmessage = e => {  // подписываем элемент на событие получения ответа
    if (!localStream) {
        console.log('not ready yet');
        return;
    }
    switch (e.data.type) {
        case 'offer':
            handleOffer(e.data);
            break;
        case 'answer':
            handleAnswer(e.data);
            break;
        case 'candidate':
            handleCandidate(e.data);
            break;
        case 'ready': // кейс когда человек нажал кнопку connect  приложение получило досутп к камере и образовал перечаду данных
            if (pc) { // если peerconnection уже есть для связи с другим клиентом то просто выходим их метода
                console.log('already in call, ignoring');
                return;
            }
            makeCall(); // инициализация соединения
            break;
        case 'bye':
            if (pc) {
                hangup();
            }
            break;
        default:
            console.log('unhandled', e);
            break;
    }
};

startButton.onclick = async () => { // метод образования связи с локальной камерой и передачи на экран своего изображения
    localStream = await navigator.mediaDevices.getUserMedia({ audio: true, video: true });
    localVideo.srcObject = localStream;


    startButton.disabled = true;
    hangupButton.disabled = false;

    signaling.postMessage({ type: 'ready' });
};

hangupButton.onclick = async () => {
    hangup();
    signaling.postMessage({ type: 'bye' });
};
async function hangup() {
    if (pc) {
        pc.close();
        pc = null;
    }
    localStream.getTracks().forEach(track => track.stop());
    localStream = null;
    startButton.disabled = false;
    hangupButton.disabled = true;
};
function createPeerConnection() {
    pc = new RTCPeerConnection();
    pc.onicecandidate = e => {
        const message = {
            type: 'candidate',
            candidate: null,
        };
        if (e.candidate) {
            message.candidate = e.candidate.candidate;
            message.sdpMid = e.candidate.sdpMid;
            message.sdpMLineIndex = e.candidate.sdpMLineIndex;
        }
        signaling.postMessage(message);
    };
    pc.ontrack = e => remoteVideo.srcObject = e.streams[0];
    localStream.getTracks().forEach(track => pc.addTrack(track, localStream));
}
async function makeCall() {
    await createPeerConnection();
    // После создания RTCPeerConnection нам необходимо создать предложение или ответ SDP, в зависимости от того, являемся ли мы вызывающим или принимающим узлом
    const offer = await pc.createOffer();
    signaling.postMessage({ type: 'offer', sdp: offer.sdp });
    await pc.setLocalDescription(offer); //описание сеанса устанавливается как локальное описание с помощью setLocalDescription()
}
async function handleOffer(offer) {
    if (pc) {
        console.error('existing peerconnection');
        return;
    }
    await createPeerConnection();
    await pc.setRemoteDescription(offer);

    const answer = await pc.createAnswer();
    signaling.postMessage({ type: 'answer', sdp: answer.sdp });
    await pc.setLocalDescription(answer);
}
async function handleAnswer(answer) {
    if (!pc) {
        console.error('no peerconnection');
        return;
    }
    await pc.setRemoteDescription(answer);
}
async function handleCandidate(candidate) {
    if (!pc) {
        console.error('no peerconnection');
        return;
    }
    if (!candidate.candidate) {
        await pc.addIceCandidate(null);
    } else {
        await pc.addIceCandidate(candidate);
    }
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////
connection.start().then(function () {       
    document.getElementById("sendButton").disabled;
}).catch(function (err) {
    return console.error(err.toString());
});