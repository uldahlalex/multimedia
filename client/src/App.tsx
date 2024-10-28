import {useEffect, useState} from "react";

interface MyObjectWithImage {
    imageUrl: string
    title: string;
}

function App() {

    const [objectsWithImage, setObjectsWithImage] = useState<MyObjectWithImage[]>([])
    const [image, setImage] = useState<File | null>(null)

    useEffect(() => {
        fetch('http://localhost:5000/objectsWithImages')
            .then(response => response.json())
            .then(data => setObjectsWithImage(data))
    }, []);

    return (
        <>
            <input onChange={e => setImage(e.target.files![0]!)} type="file"/>

            <button onClick={ async () => {
                const formData = new FormData();
                formData.append('file', image!);

                const response = await fetch('http://localhost:5000/objectsWithImages', {
                    method: 'POST',
                    body: formData
                });

                const data = await response.json()
                setObjectsWithImage([...objectsWithImage, data])

            }}>Upload image</button>

            {
                objectsWithImage.map((object, index) => (
                    <div key={index}>
                        <img src={object.imageUrl} alt={object.title}/>
                        <p>{object.title}</p>
                    </div>
                ))

            }
        </>
    )
}

export default App
