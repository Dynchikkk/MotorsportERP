import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import './styles/globals.css';
import { App } from './App';

export const renderApp = () => {
  const container = document.getElementById('root');

  if (!container) {
    throw new Error('Root container was not found');
  }

  createRoot(container).render(
    <StrictMode>
      <App />
    </StrictMode>,
  );
};
