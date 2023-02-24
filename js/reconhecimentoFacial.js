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

async function drawVideo(){
    //Desenhar o enquadramento do vídeo na tela do usuário
    ctx.drawImage(video, 0, 0);
    for (face of curFaces){
        drawFace(face);  
    } 
    //Requisitar a presença do frame de vídeo novamente
    requestAnimationFrame(drawVideo);
}
  
//Desenhando e delimitando a posição dos olhos na tela, diretamente captado no enquadramento de vídeo
async function drawFace(face){
     ctx.fillStyle = 'cyan';
      for (pt of face.scaledMesh){
          ctx.beginPath();
          ctx.ellipse(pt[0], pt[1], 3, 3, 0, 0, 2*Math.PI)
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
