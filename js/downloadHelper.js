function downloadFromUrl(url, fileName) {
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
}

function downloadFromByteArray({ byteArray, fileName, contentType }) {
    const url = URL.createObjectURL(new Blob([byteArray], { type: contentType }));
    downloadFromUrl(url, fileName);
    URL.revokeObjectURL(url);
}