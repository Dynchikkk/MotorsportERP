function App() {
  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-racing-dark">
      <h1 className="text-4xl font-bold text-white mb-4 uppercase tracking-wider">
        Motorsport <span className="text-racing-red">ERP</span>
      </h1>
      <p className="text-racing-gray mb-8">
        Платформа для настоящих гонщиков
      </p>
      <button className="bg-racing-red hover:bg-red-700 text-white font-bold py-3 px-8 rounded transition-colors">
        Поехали
      </button>
    </div>
  )
}

export default App