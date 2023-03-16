//Setup de funcionamento da câmera :) 
async function setupCamera() {
    //Encontrando o elemento/função de vídeo na página HTML
    video = document.getElementById('video');
    //Solicitando o uso da câmera frontal do dispositivo do usuário
    const stream = await navigator.mediaDevices.getUserMedia({
        'audio': false,
        //Configurações da função de vídeo, como altura e comprimento da imagem revelada ao usuário 
        'video': {
            facingMode: 'user',
            width: { ideal: 1920 },
            height: { ideal: 1080 },
        },
    });
    video.srcObject = stream;

    //Manipulando o stream de vídeo uma vez que permitido/carregado
    return new Promise((resolve) => {
        video.onloadedmetadata = () => {
            resolve(video);
        };
    });
}

//Chamando o traçamento facial para a imagem do rosto do usuário! Delimitando globalmente as posições dos pontos faciais de qualquer rosto apresentados
var curFaces = [];
async function renderPrediction() {
    const facepred = await fmesh.estimateFaces(video);

    if (facepred.length > 0) { //Realizar o procedimento caso haja a identificação de um rosto
        curFaces = facepred;
    }

    requestAnimationFrame(renderPrediction);
};

async function drawVideo() {
    //Desenhar o enquadramento do vídeo na tela do usuário
    ctx.drawImage(video, 0, 0);
    for (face of curFaces) {
        drawFace(face);
    }
    //Requisitar a presença do frame de vídeo novamente
    requestAnimationFrame(drawVideo);
}

//Desenhando e delimitando a posição dos olhos na tela, diretamente captado no enquadramento de vídeo
async function drawFace(face) {
    ctx.fillStyle = 'cyan';
    for (pt of face.scaledMesh) {
        ctx.beginPath();
        ctx.ellipse(pt[0], pt[1], 3, 3, 0, 0, 2 * Math.PI)
        ctx.fill();
    }
}

//Declarando as varíaveis para aplicação das funções na tela
var canvas;
var ctx;

//Adicionando função ao main()
async function main() {
    //Delimitando o número máximo de captação de rostos
    fmesh = await facemesh.load({ maxFaces: 3 });

    //Configurando as funções da câmera frontal
    await setupCamera();
    videoWidth = video.videoWidth;
    videoHeight = video.videoHeight;
    video.play()

    //Configurando para que o desenho do canva seja aplicado no vídeo da tela HTML
    canvas = document.getElementById('facecanvas');
    canvas.width = videoWidth;
    canvas.height = videoHeight;
    ctx = canvas.getContext('2d');

    //Declarando o loop para funcionamento do vídeo detectado pela câmera e desenho do canvas
    drawVideo()
    renderPrediction();
}

// DEFINIÇÃO DO FILTRO CRIADO PARA TESTES

// Desenhar e localizar as funções definidas para os olhos no canva
// async function drawFace(face) {
//     drawEyesBig(face);
// }

// // Definindo a função de aumento dos olhos
// function drawEyesBig(face) {
//     let mesh = face.scaledMesh;

//     // Definindo e localizando os pontos do olho esquerdo TL, BR: 27/130, 23/243
//     let lTop = mesh[27][1];
//     let lLeft = mesh[130][0];
//     let lBot = mesh[23][1];
//     let lRig = mesh[243][0];
//     let lWid = lRig - lLeft;
//     let lHei = lBot - lTop;

//     // Definindo e localizando os pontos do olho direito TL, BR: 257/463, 253/359
//     let rTop = mesh[257][1];
//     let rLeft = mesh[463][0];
//     let rBot = mesh[253][1];
//     let rRig = mesh[359][0];
//     let rWid = rRig - rLeft;
//     let rHei = rBot - rTop;

//     // Desenhar a imagem desejada dobrada de tamanho e em seguimento com o rosto
//     ctx.drawImage(video, rLeft, rTop, rWid, rHei,
//         rLeft + rWid * .05 - rWid * .5, rTop - rHei * .5, 2 * rWid, 2 * rHei);
//     ctx.drawImage(video, lLeft, lTop, lWid, lHei,
//         lLeft - lWid * .05 - lWid * .5, lTop - lHei * .5, 2 * lWid, 2 * lHei);
// }
