const formatter = new Intl.NumberFormat(undefined, {
  maximumFractionDigits: 5,
});

function isValid(value?: number): value is number {
  return value !== undefined && value !== null;
}

interface Props {
  location?: {
    latitude?: number;
    longitude?: number;
    address?: string;
  };
}

export function Location(props: Props) {
  const { location } = props;
  if (!location) {
    return null;
  }

  const { latitude, longitude, address } = location;
  return (
    <div>
      {isValid(latitude) && isValid(longitude) && (
        <div>
          {formatter.format(latitude)}, {formatter.format(longitude)}
        </div>
      )}
      {address && <div>{address}</div>}
    </div>
  );
}
