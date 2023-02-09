import { GroupItem, GroupName } from '@example/dataui';
import { useQueryAndSubscription } from './useQueryAndSubscription';

const sharedQuery = `
    name
    description
    count
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
`;

const dataQuery = `
{
  groups {
${sharedQuery}
  }
}
`;

const subscribeQuery = `
subscription {
  groupUpdated {
${sharedQuery}
  }
}
`;

interface SharedData {
  name: string;
  description: string;
  count: number;
  drivers: {
    name: string;
    driverId: string;
    transport?: {
      vehicle: {
        vehicleId: string;
        vehicleType: 'Truck' | 'Van';
      };
    };
  }[];
}

interface SubscriptionData {
  groupUpdated: SharedData;
}

interface QueryData {
  groups: SharedData[];
}

interface ResultData {
  groups: SharedData[];
}

export function Groups() {
  const [result] = useQueryAndSubscription<
    SubscriptionData,
    QueryData,
    ResultData
  >({
    query: dataQuery,
    subscription: subscribeQuery,
    onQuery: (data) => {
      return {
        groups: data.groups,
      };
    },
    onSubscribe: (data, current) => {
      if (!current) {
        current = {
          groups: [],
        };
      }
      return {
        ...current,
        groups: current.groups.reduce(
          (list, group) => {
            if (group.name !== data.groupUpdated.name) {
              list.push(group);
            }
            return list;
          },
          [data.groupUpdated]
        ),
      };
    },
  });

  if (!result) {
    return null;
  }

  const { groups } = result;
  const activeGroups = groups
    .filter((g) => g.count !== 0)
    .sort((a, b) => a.name.localeCompare(b.name));

  return (
    <div>
      {activeGroups.map((g) => (
        <GroupName
          key={g.name}
          name={g.name}
          count={g.count}
          element={g.drivers.map((d) => (
            <GroupItem
              key={d.driverId}
              driverId={d.driverId}
              driverName={d.name}
              vehicleId={d.transport?.vehicle?.vehicleId ?? '?'}
              vehicleType={d.transport?.vehicle?.vehicleType}
            />
          ))}
        />
      ))}
    </div>
  );
}

export default Groups;
