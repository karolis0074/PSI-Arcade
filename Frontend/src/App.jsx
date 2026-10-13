import Layout from './components/Layout.jsx';
import { useRoute } from './router.js';
import GamesPage from './pages/GamesPage.jsx';
import GameDetailPage from './pages/GameDetailPage.jsx';
import CreateGamePage from './pages/CreateGamePage.jsx';
import FieldsPage from './pages/FieldsPage.jsx';
import ProfilePage from './pages/ProfilePage.jsx';
import AuthPage from './pages/AuthPage.jsx';

export default function App() {
  const { segments, query } = useRoute();
  const [section = 'games', param] = segments;

  let page;
  let active = section;
  switch (section) {
    case 'games':
      page = param ? <GameDetailPage id={Number(param)} /> : <GamesPage />;
      break;
    case 'create':
      page = <CreateGamePage fieldId={query.field ? Number(query.field) : undefined} />;
      break;
    case 'fields':
      page = <FieldsPage />;
      break;
    case 'profile':
      page = <ProfilePage />;
      break;
    case 'login':
    case 'register':
      page = <AuthPage mode={section} next={query.next} />;
      active = 'profile';
      break;
    default:
      page = <GamesPage />;
      active = 'games';
  }

  return <Layout active={active}>{page}</Layout>;
}
