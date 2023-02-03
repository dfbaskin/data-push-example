import { Truck, Van } from '@example/dataui';
import { useQuery } from 'urql';
import { usePolling } from './usePolling';

const testQuery = `
{
  groups {
    name
    description
    drivers {
      name
      driverId
      transport {
        vehicle {
          vehicleId
          vehicleType
        }
      }
    }
  }
}
`;

interface Data {
  groups: {
    name: string;
    description: string;
    drivers: {
      name: string;
      driverId: string;
      transport?: {
        vehicle: {
          vehicleId: string;
          vehicleType: "Truck" | "Van";
        }
      }
    }[]
  }[]
}

const defaultData: Data = {
  groups: []
};

export function Groups() {
  const [{data}, reexecuteQuery] = useQuery<Data>({
    query: testQuery,
    requestPolicy: 'network-only'
  });
  usePolling(() => {
    reexecuteQuery();
  });

  const groups = (data ?? defaultData).groups;
  const activeGroups = groups.filter(g => g.drivers.length !== 0);

  const getIcon = (driver: Data["groups"][0]["drivers"][0]) => {
    switch(driver.transport?.vehicle?.vehicleType) {
      case "Truck":
        return <Truck />;
      case "Van":
        return <Van />;
    }
    return null;
  }

  return (
    <div>
      <ul>

      {activeGroups.map(g => (
        <li key={g.name}>
          <span title={g.description}>
            {g.name}
            ({g.drivers.length})
          </span>
          <ul>
            {g.drivers.map(d => (
              <li key={d.driverId}>
                {getIcon(d)}
                {d.driverId} /
                {d.transport?.vehicle.vehicleId}
                ({d.name})
              </li>
            ))}
          </ul>
        </li>
      ))}
      </ul>
    </div>
  );
}

export default Groups;
